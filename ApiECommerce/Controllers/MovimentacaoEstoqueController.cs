using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovimentacaoEstoqueController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public MovimentacaoEstoqueController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // Registrar uma movimentação 
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MovimentarEstoque([FromBody] MovimentacaoEstoque movimentacao)
    {
        var produtoEstoque = await dbContext.Estoque.FirstOrDefaultAsync(e => e.ProdutoId == movimentacao.ProdutoId);

        if (movimentacao.Tipo == TipoMovimentacao.Venda)
        {
            // Se for venda, precisa ter estoque
            if (produtoEstoque == null /*|| produtoEstoque.Quantidade < movimentacao.Quantidade*/)
                return BadRequest("Estoque inexistente para este produto.");

            produtoEstoque.Quantidade -= movimentacao.Quantidade;
        }
        else if (movimentacao.Tipo == TipoMovimentacao.Compra)
        {
            if (produtoEstoque == null)
            {
                // Cria novo registro de estoque
                produtoEstoque = new Estoque
                {
                    ProdutoId = movimentacao.ProdutoId,
                    Quantidade = movimentacao.Quantidade,
                    DataEntrada = DateTime.UtcNow,
                    PrecoCusto = movimentacao.PrecoCusto
                };
                dbContext.Estoque.Add(produtoEstoque);
            }
            else
            {
                // Atualiza o existente
                produtoEstoque.Quantidade += movimentacao.Quantidade;
                produtoEstoque.DataEntrada = DateTime.UtcNow;
                produtoEstoque.PrecoCusto = movimentacao.PrecoCusto;
            }
        }

        // Registra movimentação
        dbContext.MovimentacoesEstoque.Add(movimentacao);
        await dbContext.SaveChangesAsync();

        return Ok(movimentacao);
    }

    // ✅ Consultar todas as movimentações
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimentacaoEstoque>>> GetMovimentacoes()
    {
        return await dbContext.MovimentacoesEstoque
            .Include(m => m.Produto)
            .ToListAsync();
    }
}
