using ApiECommerce.Context;
using ApiECommerce.DTOs;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItensComandaController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public ItensComandaController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: api/ItensComanda/1  Pega os itens da comanda de um cliente que está aberta
    [HttpGet("{nome}")]
    public async Task<IActionResult> Get(string nome)
    {
        var itensComanda = await (from s in dbContext.ItensComanda
                                  join p in dbContext.Produtos on s.ProdutoId equals p.Id // Join com a tabela Produtos
                                  where s.Nome == nome // Filtro por identificação da comanda
                                  select new ItemComandaDTO
                                  {
                                      Id = s.Id,
                                      PrecoUnitario = s.PrecoUnitario,
                                      Quantidade = s.Quantidade,
                                      ProdutoId = s.ProdutoId,
                                      Nome = s.Nome,
                                      NomeProduto = p.Nome,
                                  }).ToListAsync();

        return Ok(itensComanda);
    }

    // POST: api/ItensComanda
    // Este método Action trata de uma requisição HTTP do tipo POST para adicionar um
    // novo item ao ItensComanda ou atualizar a quantidade de um item existente
    // no carrinho. Ele verifica se o item já está na ItensComanda com base no ID do produto
    // e no ID do cliente. Se o item já estiver no carrinho, sua quantidade é atualizada
    // e o valor total é recalculado. Caso contrário, um novo item é adicionado a comanda
    // com as informações fornecidas. Após as operações no banco de dados, o método retorna
    // um código de status 201 (Created).
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ItemComandaDTO itemComanda)
    {
        try
        {
            // Busca a comanda pelo nome
            var comanda = await dbContext.Comandas.FirstOrDefaultAsync(c => c.Nome == itemComanda.Nome);
            
            if (comanda == null)
            {
                return NotFound($"Comanda '{itemComanda.Nome}' não encontrada.");
            }

            var itemNaComanda = await dbContext.ItensComanda
                                .FirstOrDefaultAsync(s =>
                                s.ProdutoId == itemComanda.ProdutoId &&
                                s.ComandaId == comanda.Id);

            if (itemNaComanda is not null)
            {
                // Se o item já existe, apenas aumenta a quantidade
                itemNaComanda.Quantidade += itemComanda.Quantidade;
            }
            else
            {
                // Cria um novo item e associa à comanda através do ComandaId
                var novoItem = new ItemComanda()
                {
                    PrecoUnitario = itemComanda.PrecoUnitario,
                    Quantidade = itemComanda.Quantidade,
                    ProdutoId = itemComanda.ProdutoId,
                    Nome = itemComanda.Nome,
                    ComandaId = comanda.Id // ✅ Define o ComandaId para estabelecer o relacionamento
                };
                dbContext.ItensComanda.Add(novoItem);
            }

            // Atualiza o valor total da comanda
            await dbContext.SaveChangesAsync();
            
            // Recalcula o valor total da comanda
            comanda.ValorTotal = await dbContext.ItensComanda
                .Where(i => i.ComandaId == comanda.Id)
                .SumAsync(i => i.PrecoUnitario * i.Quantidade);
            
            await dbContext.SaveChangesAsync();
            
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Ocorreu um erro ao processar a solicitação: {ex.Message}");
        }
    }

    // PUT /api/DetalhesComanda?produtoId = 1 & acao = "aumentar"
    // PUT /api/DetalhesComanda?produtoId = 1 & acao = "diminuir"
    // PUT /api/DetalhesComanda?produtoId = 1 & acao = "deletar"
    //--------------------------------------------------------------------
    // Este código manipula itens no carrinho de compras de um usuário com base em uma
    // ação("aumentar", "diminuir" ou "deletar") e um ID de produto.
    // Obtém o usuário logado:
    //    Usa o e-mail do usuário logado para buscar o usuário no banco de dados.
    // Busca o item do carrinho do produto:
    // Procura o item no carrinho com base no ID do produto e no ID do cliente (usuário logado).
    // Realiza a ação especificada:
    //    Aumentar:
    //        Se a quantidade for maior que 0, aumenta a quantidade do item em 1.
    //    Diminuir:
    //        Se a quantidade for maior que 1, diminui a quantidade do item em 1.
    //        Se a quantidade for 1, remove o item do carrinho.
    //    Deletar:
    //        Remove o item do carrinho.
    // Atualiza o valor total do item:
    //    Multiplica o preço unitário pela quantidade, atualizando o valor total do item no carrinho.
    // Salva as alterações no banco de dados:
    //    Salva as alterações feitas no item do carrinho no banco de dados.
    // Retorna o resultado:
    //    Se a ação for bem-sucedida, retorna "Ok".
    //    Se o item não for encontrado, retorna "NotFound".
    //    Se a ação for inválida, retorna "BadRequest".
    /// <summary>
    /// Atualiza a quantidade de um item no carrinho de compras do usuário.
    /// </summary>
    /// <param name="produtoId">O ID do produto.</param>
    /// <param name="acao">A ação a ser realizada no item do carrinho. Opções: 'aumentar', 'diminuir' ou 'deletar'.</param>
    /// <returns>Um objeto IActionResult representando o resultado da operação.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(int produtoId, string acao, string nome)
    {
        // Busca a comanda pelo nome
        var comanda = await dbContext.Comandas.FirstOrDefaultAsync(c => c.Nome == nome);
        
        if (comanda == null)
        {
            return NotFound($"Comanda '{nome}' não encontrada.");
        }

        var itemComanda = await dbContext.ItensComanda.FirstOrDefaultAsync(s =>
                                               s.ComandaId == comanda.Id && s.ProdutoId == produtoId);

        if (itemComanda != null)
        {
            if (acao.ToLower() == "aumentar")
            {
                itemComanda.Quantidade += 1;
            }
            else if (acao.ToLower() == "diminuir")
            {
                if (itemComanda.Quantidade > 1)
                {
                    itemComanda.Quantidade -= 1;
                }
                else
                {
                    dbContext.ItensComanda.Remove(itemComanda);
                    await dbContext.SaveChangesAsync();
                    
                    // Recalcula o valor total da comanda após remover o item
                    comanda.ValorTotal = await dbContext.ItensComanda
                        .Where(i => i.ComandaId == comanda.Id)
                        .SumAsync(i => i.PrecoUnitario * i.Quantidade);
                    
                    await dbContext.SaveChangesAsync();
                    return Ok("Item removido com sucesso.");
                }
            }
            else if (acao.ToLower() == "deletar")
            {
                // Remove o item do carrinho
                dbContext.ItensComanda.Remove(itemComanda);
                await dbContext.SaveChangesAsync();
                
                // Recalcula o valor total da comanda após remover o item
                comanda.ValorTotal = await dbContext.ItensComanda
                    .Where(i => i.ComandaId == comanda.Id)
                    .SumAsync(i => i.PrecoUnitario * i.Quantidade);
                
                await dbContext.SaveChangesAsync();
                return Ok("Item deletado com sucesso.");
            }
            else
            {
                return BadRequest("Ação Inválida. Use : 'aumentar', 'diminuir', ou 'deletar' para realizar uma ação");
            }

            // Recalcula o valor total da comanda
            comanda.ValorTotal = await dbContext.ItensComanda
                .Where(i => i.ComandaId == comanda.Id)
                .SumAsync(i => i.PrecoUnitario * i.Quantidade);

            await dbContext.SaveChangesAsync();

            return Ok($"Operacao : {acao} realizada com sucesso");
        }
        else
        {
            return NotFound("Nenhum item encontrado na comanda");
        }
    }
}
