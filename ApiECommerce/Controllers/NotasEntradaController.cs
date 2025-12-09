using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotasEntradaController : ControllerBase
{
    private readonly AppDbContext _db;

    public NotasEntradaController(AppDbContext db)
    {
        _db = db;
    }

    // Recebe XML no corpo da requisição e processa a nota de entrada
    [HttpPost]
    [Consumes("application/xml")]
    public async Task<IActionResult> ImportarXml()
    {
        string xml;
       
        using (var reader = new StreamReader(Request.Body))
        {
            xml = await reader.ReadToEndAsync();
        }

        // já leu o corpo em 'xml'
        var contentType = Request.ContentType ?? string.Empty;

        // se foi enviado como JSON string (escaped) ou content-type = application/json, desserialize para obter o XML cru
        if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(xml) && xml.TrimStart().StartsWith('"')))
        {
            try
            {
                xml = System.Text.Json.JsonSerializer.Deserialize<string>(xml) ?? xml;
            }
            catch
            {
                // fallback: tenta remover aspas extremas e unescape simples
                xml = xml.Trim();
                if (xml.Length >= 2 && xml[0] == '\"' && xml[^1] == '\"')
                    xml = xml.Substring(1, xml.Length - 2);
                xml = System.Text.RegularExpressions.Regex.Unescape(xml);
            }
        }

        if (string.IsNullOrWhiteSpace(xml))
            return BadRequest("XML vazio.");

        // Se o corpo veio como string JSON com o XML entre aspas, remove aspas e unescape
        if (xml.StartsWith("\"") && xml.EndsWith("\""))
        {
            xml = xml.Substring(1, xml.Length - 2);
            xml = Regex.Unescape(xml);
        }

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml);
        }
        catch (Exception ex)
        {
            return BadRequest($"XML inválido: {ex.Message}");
        }

        // NF-e usa namespace - tenta primeiro com namespace, se não, busca pelo LocalName
        XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
        var infNFe = doc.Descendants(ns + "infNFe").FirstOrDefault()
                    ?? doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "infNFe");
        if (infNFe == null) return BadRequest("XML sem elemento infNFe.");

        // Extrai campos básicos do grupo infNFe / ide / emit / total
        var numero = infNFe.Element(ns + "ide")?.Element(ns + "nNF")?.Value
                     ?? infNFe.Elements().FirstOrDefault(e => e.Name.LocalName == "ide")?
                        .Elements().FirstOrDefault(e => e.Name.LocalName == "nNF")?.Value;

        var fornecedor = infNFe.Element(ns + "emit")?.Element(ns + "xNome")?.Value
                        ?? infNFe.Elements().FirstOrDefault(e => e.Name.LocalName == "emit")?
                            .Elements().FirstOrDefault(e => e.Name.LocalName == "xNome")?.Value;

        var dataEmissaoStr = infNFe.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value
                            ?? infNFe.Elements().FirstOrDefault(e => e.Name.LocalName == "ide")?
                                .Elements().FirstOrDefault(e => e.Name.LocalName == "dhEmi")?.Value;

        var valorTotalStr = infNFe.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vNF")?.Value
                          ?? infNFe.Elements().FirstOrDefault(e => e.Name.LocalName == "total")?
                                .Elements().FirstOrDefault(e => e.Name.LocalName == "ICMSTot")?
                                .Elements().FirstOrDefault(e => e.Name.LocalName == "vNF")?.Value;

        DateTime dataEmissao = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(dataEmissaoStr))
        {
            // tenta parse com estilos diversos e removendo timezone se necessário
            if (!DateTime.TryParse(dataEmissaoStr, null, DateTimeStyles.AdjustToUniversal, out var dt))
            {
                // tenta sem timezone
                var s = Regex.Replace(dataEmissaoStr, "[+-][0-9]{2}:[0-9]{2}$", "");
                DateTime.TryParse(s, out dt);
            }
            dataEmissao = dt;
        }

        decimal? valorTotal = null;
        if (!string.IsNullOrWhiteSpace(valorTotalStr))
        {
            if (decimal.TryParse(valorTotalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var vt))
                valorTotal = vt;
            else
            {
                // tenta com vírgula
                var alt = valorTotalStr.Replace(',', '.');
                if (decimal.TryParse(alt, NumberStyles.Any, CultureInfo.InvariantCulture, out vt))
                    valorTotal = vt;
            }
        }

        var nota = new NotaEntrada
        {
            NumeroNota = numero,
            Fornecedor = fornecedor,
            DataEmissao = dataEmissao,
            ValorTotal = valorTotal
        };

        // coleta elementos det/prod que representam itens - no padrão NF-e são os elementos <det>
        var detElements = infNFe.Elements(ns + "det").Any()
            ? infNFe.Elements(ns + "det")
            : infNFe.Elements().Where(e => e.Name.LocalName == "det");

        var itemElements = detElements;

        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            foreach (var itemEl in itemElements)
            {
                // em NF-e, dentro de <det> existe <prod> com campos como xProd, cEAN, qCom, vUnCom
                var prodEl = itemEl.Element(ns + "prod") ?? itemEl.Elements().FirstOrDefault(e => e.Name.LocalName == "prod");
                var nome = prodEl?.Element(ns + "xProd")?.Value ?? prodEl?.Elements().FirstOrDefault(e => e.Name.LocalName == "xProd")?.Value;
                var barcode = prodEl?.Element(ns + "cEAN")?.Value ?? prodEl?.Elements().FirstOrDefault(e => e.Name.LocalName == "cEAN")?.Value;
                var qtdStr = prodEl?.Element(ns + "qCom")?.Value ?? prodEl?.Elements().FirstOrDefault(e => e.Name.LocalName == "qCom")?.Value;
                var precoCustoStr = prodEl?.Element(ns + "vUnCom")?.Value ?? prodEl?.Elements().FirstOrDefault(e => e.Name.LocalName == "vUnCom")?.Value;

                if (string.IsNullOrWhiteSpace(nome) && string.IsNullOrWhiteSpace(barcode))
                    continue; // ignora

                int quantidade = 0;
                if (!string.IsNullOrWhiteSpace(qtdStr))
                {
                    // qCom pode estar em formato decimal (ex: 3.0000)
                    if (decimal.TryParse(qtdStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var qd))
                        quantidade = (int)qd;
                    else if (int.TryParse(qtdStr, out var qi))
                        quantidade = qi;
                }

                decimal? precoCusto = null;
                if (!string.IsNullOrWhiteSpace(precoCustoStr))
                {
                    if (decimal.TryParse(precoCustoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var pc))
                        precoCusto = pc;
                    else if (decimal.TryParse(precoCustoStr.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out pc))
                        precoCusto = pc;
                }

                // Localiza produto por barcode ou nome
                Produto? produto = null;
                if (!string.IsNullOrWhiteSpace(barcode))
                    produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Barcode == barcode);

                if (produto == null && !string.IsNullOrWhiteSpace(nome))
                    produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Nome == nome);

                if (produto == null)
                {
                    // Cria produto padrão (atribuir CategoriaId padrão 21 - Outros)
                    produto = new Produto
                    {
                        Nome = nome,
                        Barcode = barcode,
                        Preco = precoCusto ?? 0m,
                        CategoriaId = 21,
                        EmEstoque = quantidade,
                        Disponivel = true
                    };
                    _db.Produtos.Add(produto);
                    await _db.SaveChangesAsync();
                }

                // Atualiza ou cria Estoque
                var estoque = await _db.Estoque.FirstOrDefaultAsync(e => e.ProdutoId == produto.Id);
                if (estoque == null)
                {
                    estoque = new Estoque
                    {
                        ProdutoId = produto.Id,
                        Quantidade = quantidade,
                        DataEntrada = DateTime.UtcNow,
                        PrecoCusto = precoCusto
                    };
                    _db.Estoque.Add(estoque);
                }
                else
                {
                    estoque.Quantidade += quantidade;
                    estoque.DataEntrada = DateTime.UtcNow;
                    if (precoCusto.HasValue)
                        estoque.PrecoCusto = precoCusto;
                }

                // Atualiza campo EmEstoque no produto
                produto.EmEstoque = (estoque?.Quantidade) ?? produto.EmEstoque;

                // Registra movimentação de compra
                var mov = new MovimentacaoEstoque
                {
                    ProdutoId = produto.Id,
                    Quantidade = quantidade,
                    PrecoCusto = precoCusto,
                    Tipo = TipoMovimentacao.Compra,
                    DataMovimentacao = DateTime.UtcNow
                };
                _db.MovimentacoesEstoque.Add(mov);

                // Adiciona item na nota
                var notaItem = new NotaEntradaItem
                {
                    ProdutoId = produto.Id,
                    Nome = produto.Nome,
                    Barcode = produto.Barcode,
                    Quantidade = quantidade,
                    PrecoCusto = precoCusto
                };
                nota.Itens.Add(notaItem);

                // Salva alterações parciais para garantir ids
                await _db.SaveChangesAsync();
            }

            _db.NotasEntrada.Add(nota);
            await _db.SaveChangesAsync();

            await tx.CommitAsync();

            return CreatedAtAction(nameof(GetNota), new { id = nota.Id }, nota);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return BadRequest($"Erro ao processar XML: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNota(int id)
    {
        var nota = await _db.NotasEntrada
            .Include(n => n.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (nota == null) return NotFound();
        return Ok(nota);
    }

    // GET: api/NotasEntrada
    // Retorna todas as notas com seus itens e informações de produto
    [HttpGet]
    public async Task<IActionResult> GetTodasNotas()
    {
        var notas = await _db.NotasEntrada
            .Include(n => n.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(n => n.DataEmissao)
            .Select(n => new {
                n.Id,
                n.NumeroNota,
                n.DataEmissao,
                n.Fornecedor,
                n.ValorTotal,
                Itens = n.Itens.Select(i => new {
                    i.Id,
                    i.NotaEntradaId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto != null ? i.Produto.Nome : i.Nome,
                    i.Nome,
                    i.Barcode,
                    i.Quantidade,
                    i.PrecoCusto
                }).ToList()
            }).ToListAsync();

        return Ok(notas);
    }
}
