using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EstoqueController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public EstoqueController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: Consultar estoque de todos os produtos
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Estoque>>> GetEstoques()
    {
        return await dbContext.Estoque
            .Include(e => e.Produto)
            .Where(e => e.Quantidade != 0)
            .ToListAsync();
    }

    // GET: Consultar estque de um produto específico
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Estoque>> GetProdutoEstoque(int id)
    {
        var estoque = await dbContext.Estoque
            .Include(e => e.Produto)
            .FirstOrDefaultAsync(e  => e.ProdutoId == id);

        if(estoque == null)
            return NotFound();

        return estoque;
    }


    // POST: EstoqueController/Edit/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEstoque(int id, [FromBody] Estoque estoque)
    {
        if(id != estoque.Id) 
            return BadRequest();

        dbContext.Entry(estoque).State = EntityState.Modified;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!dbContext.Estoque.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // PUT: api/Estoque/barcode/{barcode}
    // Atualiza quantidade e preço de custo do estoque usando código de barras
    [HttpPut("barcode/{barcode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEstoquePorBarcode(string barcode, [FromBody] EstoqueUpdateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return BadRequest(new
            {
                sucesso = false,
                mensagem = "Código de barras não pode ser vazio."
            });
        }

        if (dto == null)
        {
            return BadRequest(new
            {
                sucesso = false,
                mensagem = "Dados de atualização inválidos."
            });
        }

        try
        {
            // Busca o produto pelo barcode
            var produto = await dbContext.Produtos
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

            if (produto == null)
            {
                return NotFound(new
                {
                    sucesso = false,
                    barcode = barcode,
                    mensagem = "Produto não encontrado com este código de barras."
                });
            }

            // Busca ou cria o estoque do produto
            var estoque = await dbContext.Estoque
                .FirstOrDefaultAsync(e => e.ProdutoId == produto.Id);

            if (estoque == null)
            {
                // Cria novo estoque se não existir
                estoque = new Estoque
                {
                    ProdutoId = produto.Id,
                    Quantidade = dto.Quantidade ?? 0,
                    PrecoCusto = dto.PrecoCusto,
                    DataEntrada = DateTime.UtcNow,
                    Ativo = true
                };
                dbContext.Estoque.Add(estoque);
            }
            else
            {
                // Atualiza estoque existente
                if (dto.Quantidade.HasValue)
                {
                    estoque.Quantidade = dto.Quantidade.Value;
                }

                if (dto.PrecoCusto.HasValue)
                {
                    estoque.PrecoCusto = dto.PrecoCusto.Value;
                }

                estoque.DataEntrada = DateTime.UtcNow;
            }

            // Atualiza EmEstoque no produto
            produto.EmEstoque = estoque.Quantidade;

            // Registra movimentação se houver quantidade
            if (dto.Quantidade.HasValue && dto.Quantidade.Value > 0)
            {
                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = produto.Id,
                    Quantidade = dto.Quantidade.Value,
                    PrecoCusto = dto.PrecoCusto,
                    Tipo = TipoMovimentacao.Compra,
                    DataMovimentacao = DateTime.UtcNow
                };
                dbContext.MovimentacoesEstoque.Add(movimentacao);
            }

            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                sucesso = true,
                mensagem = "Estoque atualizado com sucesso.",
                produto = new
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Barcode = produto.Barcode
                },
                estoque = new
                {
                    Id = estoque.Id,
                    Quantidade = estoque.Quantidade,
                    PrecoCusto = estoque.PrecoCusto,
                    DataEntrada = estoque.DataEntrada
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                sucesso = false,
                mensagem = "Ocorreu um erro ao processar sua solicitação.",
                erro = ex.Message
            });
        }
    }
}

// DTO para atualização de estoque
public class EstoqueUpdateDTO
{
    public int? Quantidade { get; set; }
    public decimal? PrecoCusto { get; set; }
}
