using ApiECommerce.Entities;
using ApiECommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;


namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProdutosController(IProdutoRepository produtoRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _produtoRepository = produtoRepository;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetProdutos(string tipoProduto, int? categoriaId = null)
    {
        IEnumerable<Produto> produtos;

        if (tipoProduto == "categoria" && categoriaId != null)
        {
            produtos = await _produtoRepository.ObterProdutosPorCategoriaAsync(categoriaId.Value);
        }
        else if (tipoProduto == "popular")
        {
            produtos = await _produtoRepository.ObterProdutosPopularesAsync();
        }
        else if (tipoProduto == "maisvendido")
        {
            produtos = await _produtoRepository.ObterProdutosMaisVendidosAsync();
        }
        else
        {
            return BadRequest("Tipo de produto inválido");
        }

        var dadosProduto = produtos.Select(v => new
        {
            Id = v.Id,
            Nome = v.Nome,
            Preco = v.Preco,
            UrlImagem = v.UrlImagem,
            CategoriaId = v.CategoriaId
        });

        return Ok(dadosProduto);
    }

    // POST: api/Produtos/barcodelookup/consulta
    [HttpPost("barcodelookup/consulta")]
    public async Task<IActionResult> BarcodeLookupConsulta([FromBody] JsonElement payload, [FromQuery] string? token = null, [FromQuery] string? gtin = null)
    {
        // Accept payload as:
        // - a plain JSON string: "0123456789012"
        // - a JSON number: 1234567890123
        // - an object with property Barcode (or barcode/gtin)
        string? barcode = null;

        if (payload.ValueKind == JsonValueKind.String)
        {
            barcode = payload.GetString();
        }
        else if (payload.ValueKind == JsonValueKind.Number)
        {
            // numeric GTINs may be sent as numbers
            if (payload.TryGetInt64(out var num)) barcode = num.ToString();
            else barcode = payload.GetRawText();
        }
        else if (payload.ValueKind == JsonValueKind.Object)
        {
            if (payload.TryGetProperty("Barcode", out var barcodeProp) && barcodeProp.ValueKind == JsonValueKind.String)
            {
                barcode = barcodeProp.GetString();
            }
            else if (payload.TryGetProperty("barcode", out var barcodeProp2) && barcodeProp2.ValueKind == JsonValueKind.String)
            {
                barcode = barcodeProp2.GetString();
            }
            else if (payload.TryGetProperty("gtin", out var gtinProp) && gtinProp.ValueKind == JsonValueKind.String)
            {
                barcode = gtinProp.GetString();
            }
            else if (payload.TryGetProperty("Barcode", out var barcodeNumProp) && barcodeNumProp.ValueKind == JsonValueKind.Number && barcodeNumProp.TryGetInt64(out var num2))
            {
                barcode = num2.ToString();
            }
        }
        else
        {
            return BadRequest("Payload inválido. Envie string simples ou objeto com propriedade 'Barcode'.");
        }

        if (string.IsNullOrWhiteSpace(barcode)) return BadRequest("Barcode vazio.");

        // If gtin provided via query string, prefer it
        if (!string.IsNullOrWhiteSpace(gtin)) barcode = gtin;

        var client = _httpClientFactory.CreateClient();
        // Support configurable base URL for barcode service (defaults to Cosmos API)
        var baseUrl = _configuration["BarcodeLookupBaseUrl"] ?? "https://api.cosmos.bluesoft.com.br";

        string requestUrl;
        // token priority: query param -> payload property -> config
        var configuredToken = _configuration["BarcodeLookupToken"]; // optional
        string? effectiveToken = token ?? configuredToken;

        // If payload included a token property and no query token, try to read it
        if (string.IsNullOrEmpty(effectiveToken) && payload.ValueKind == JsonValueKind.Object)
        {
            if (payload.TryGetProperty("token", out var tokenProp) && tokenProp.ValueKind == JsonValueKind.String)
                effectiveToken = tokenProp.GetString();
            else if (payload.TryGetProperty("Token", out var tokenProp2) && tokenProp2.ValueKind == JsonValueKind.String)
                effectiveToken = tokenProp2.GetString();
        }

        if (baseUrl.Contains("{barcode}") || baseUrl.Contains("{token}"))
        {
            requestUrl = baseUrl.Replace("{barcode}", Uri.EscapeDataString(barcode));
            if (!string.IsNullOrEmpty(effectiveToken)) requestUrl = requestUrl.Replace("{token}", Uri.EscapeDataString(effectiveToken));
        }
        else if (baseUrl.Contains("cosmos.bluesoft.com.br", StringComparison.OrdinalIgnoreCase))
        {
            // Cosmos API: GET /gtins/{codigo}.json
            // token is required for Cosmos
            if (string.IsNullOrEmpty(effectiveToken))
            {
                return BadRequest("Cosmos API requires token. Provide ?token= or set BarcodeLookupToken in configuration.");
            }

            requestUrl = baseUrl.TrimEnd('/') + "/gtins/" + Uri.EscapeDataString(barcode) + ".json";
            // set header for token
            client.DefaultRequestHeaders.Remove("X-Cosmos-Token");
            client.DefaultRequestHeaders.Add("X-Cosmos-Token", effectiveToken);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Cosmos-API-Request");
        }
        else if (baseUrl.Contains("?"))
        {
            // append parameters; try to follow consultagtin pattern: token and in (gtin)
            requestUrl = baseUrl + (baseUrl.EndsWith("?") ? string.Empty : "&") + "in=" + Uri.EscapeDataString(barcode);
            if (!string.IsNullOrEmpty(effectiveToken)) requestUrl += "&token=" + Uri.EscapeDataString(effectiveToken);
        }
        else
        {
            // default fallback (consultagtin style)
            requestUrl = baseUrl.TrimEnd('/') + "/aplicacao/api/api.php?in=" + Uri.EscapeDataString(barcode);
            if (!string.IsNullOrEmpty(effectiveToken)) requestUrl += "&token=" + Uri.EscapeDataString(effectiveToken);
        }

            try
            {
                var resp = await client.GetAsync(requestUrl);
                if (!resp.IsSuccessStatusCode)
                {
                    var txt = await resp.Content.ReadAsStringAsync();
                    return StatusCode((int)resp.StatusCode, txt);
                }

                var content = await resp.Content.ReadAsStringAsync();

                // Try parse JSON; if content is not JSON, fallback to simple HTML/text parsing
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    // Cosmos response handling: fields like description, brand.name, thumbnail, avg_price
                    var result = new BarcodeLookupResultExternal();

                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("description", out var desc) && desc.ValueKind == JsonValueKind.String)
                            result.Title = desc.GetString();

                        // brand.name
                        if (root.TryGetProperty("brand", out var brand) && brand.ValueKind == JsonValueKind.Object && brand.TryGetProperty("name", out var bname) && bname.ValueKind == JsonValueKind.String)
                            result.Description = bname.GetString();

                        // thumbnail
                        if (root.TryGetProperty("thumbnail", out var thumb) && thumb.ValueKind == JsonValueKind.String)
                            result.Images = new List<string?> { thumb.GetString() }.Where(s => s != null).Cast<string>().ToList();

                        // price: avg_price (number) or price (string)
                        if (root.TryGetProperty("avg_price", out var avg) && avg.ValueKind == JsonValueKind.Number && avg.TryGetDecimal(out var avd))
                            result.Price = avd;
                        else if (root.TryGetProperty("price", out var pstr) && pstr.ValueKind == JsonValueKind.String)
                        {
                            // price like "R$ 2,99"
                            var s = pstr.GetString();
                            if (!string.IsNullOrWhiteSpace(s))
                            {
                                var cleaned = System.Text.RegularExpressions.Regex.Replace(s, "[^0-9,.-]", "");
                                cleaned = cleaned.Replace(',', '.');
                                if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var pd))
                                    result.Price = pd;
                            }
                        }
                    }

                    var normalized = new
                    {
                        Title = result.Title,
                        ProductName = result.Title,
                        Description = result.Description,
                        Images = result.Images,
                        Offers = new[] { new { Price = result.Price } }
                    };

                    var allNull = normalized.Title == null && normalized.Description == null && (normalized.Images == null || !normalized.Images.Any()) && (normalized.Offers == null || normalized.Offers.All(o => o.Price == null));
                    if (allNull)
                    {
                        return Ok(new { RequestUrl = requestUrl, Raw = content, Normalized = normalized });
                    }

                    return Ok(normalized);
                }
                catch (JsonException)
                {
                    // fallback: content is not JSON. Try extract from HTML or return raw text
                    string title = null;
                    string desc = null;
                    List<string> images = null;

                    try
                    {
                        // extract <title>
                        var m = System.Text.RegularExpressions.Regex.Match(content, "<title>(.*?)</title>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                        if (m.Success) title = System.Net.WebUtility.HtmlDecode(m.Groups[1].Value).Trim();

                        // meta description
                        var m2 = System.Text.RegularExpressions.Regex.Match(content, @"<meta\s+name=""description""\s+content=""(.*?)""", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
                        if (m2.Success) desc = System.Net.WebUtility.HtmlDecode(m2.Groups[1].Value).Trim();

                        // first image src
                        var m3 = System.Text.RegularExpressions.Regex.Match(content, @"<img[^>]+src=""([^""]+)""", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (m3.Success) images = new List<string> { m3.Groups[1].Value };
                    }
                    catch { }

                    var normalized = new
                    {
                        Title = title ?? content.Substring(0, Math.Min(200, content.Length)),
                        ProductName = title ?? string.Empty,
                        Description = desc,
                        Images = images,
                        Offers = new[] { new { Price = (decimal?)null } }
                    };

                    return Ok(normalized);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao consultar serviço externo: " + ex.Message);
            }
    }

    private class BarcodeLookupResultExternal
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Images { get; set; }
        public decimal? Price { get; set; }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalheProduto(int id)
    {
        var produto = await _produtoRepository.ObterDetalheProdutoAsync(id);

        if (produto is null)
        {
            return NotFound($"Produto com id={id} não encontrado");
        }

        var dadosProduto = new
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            Detalhe = produto.Detalhe,
            UrlImagem = produto.UrlImagem
        };

        return Ok(dadosProduto);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTodosProdutos()
    {
        IEnumerable<Produto> produtos;

        produtos = await _produtoRepository.ObterTodosProdutosAsync();

        var dadosProduto = produtos.Select(v => new
        {
            Id = v.Id,
            Nome = v.Nome,
            Preco = v.Preco,
            Barcode = v.Barcode,
            UrlImagem = v.UrlImagem,
            CategoriaId = v.CategoriaId,
            Popular = v.Popular,
            DiasDisponiveis = v.DiasDisponiveis,
            Disponivel = v.Disponivel
        });

        return Ok(dadosProduto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
    {
        if (produto == null || id != produto.Id)
        {
            return BadRequest("Os dados do produto são inválidos ou o ID não corresponde.");
        }

        try
        {
            produto.AtualizarDisponibilidade(); // 👈 calcula antes de salvar
            await _produtoRepository.AtualizarProdutoAsync(produto);
            return Ok(new { Message = "Produto atualizado com sucesso!", produto });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AdicionarProduto([FromBody] Produto novoProduto)
    {
        if (novoProduto == null)
        {
            return BadRequest("Os dados do produto são inválidos.");
        }

        try
        {
            novoProduto.AtualizarDisponibilidade(); // 👈 calcula antes de salvar
            await _produtoRepository.AdicionarProdutoAsync(novoProduto);
            return CreatedAtAction(nameof(GetDetalheProduto), new { id = novoProduto.Id }, novoProduto);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        try
        {
            var produto = await _produtoRepository.ObterDetalheProdutoAsync(id);
            if (produto == null)
            {
                return NotFound($"Produto com id={id} não encontrado.");
            }

            await _produtoRepository.ExcluirProdutoAsync(id);
            return NoContent(); // Retorna 204 No Content
        }
        catch (Exception ex)
        {
            // Aqui você pode logar o erro conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpGet("[action]/{topN}")]
    public async Task<IActionResult> AtualizarMaisVendidos(int topN)
    {
        if (topN == null)
        {
            return BadRequest("Insira quantos produtos.");
        }

        try
        {
            await _produtoRepository.AtualizarMaisVendidosAsync(topN);
            return Ok(new { Message = "Mais vendidos atualizado com sucesso!"});
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> MaisVendidos()
    {
        try
        {
            IEnumerable<Produto> produtos;

            produtos = await _produtoRepository.ObterProdutosMaisVendidosAsync();

            var dadosProduto = produtos.Select(v => new
            {
                Id = v.Id,
                Nome = v.Nome,
                Preco = v.Preco,
                Barcode = v.Barcode,
                UrlImagem = v.UrlImagem,
                CategoriaId = v.CategoriaId,
                Popular = v.Popular,
                DiasDisponiveis = v.DiasDisponiveis,
                Disponivel = v.Disponivel
            });

            return Ok(dadosProduto);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpPut("{id}/foto")]
    public async Task<IActionResult> AtualizarFotoProduto(int id, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo inválido.");

        using var image = await SixLabors.ImageSharp.Image.LoadAsync(arquivo.OpenReadStream()); // SixLabors.ImageSharp
        image.Mutate(x => x.Resize(225, 225));

        var nomeArquivo = $"produto_{id}.jpg";
        var caminho = Path.Combine("wwwroot", nomeArquivo);

        await image.SaveAsJpegAsync(caminho);

        // Atualize no banco (ex: produto.CaminhoImagem = $"/imagens/produtos/{nomeArquivo}")
        await _produtoRepository.AtualizarImagemProdutoAsync(id, $"/{nomeArquivo}");

        return Ok();
    }

        

}
