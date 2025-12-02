using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DespesasController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public DespesasController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    //// Obtêm todos as despesas 
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var despesas = await dbContext.Despesas
                            .OrderByDescending(despesa => despesa.Data)
                            .Select(despesa => new
                            {
                                Id = despesa.Id,
                                DataSelecionada = despesa.Data,
                                Descricao = despesa.Descricao,
                                Categoria = despesa.Categoria,
                                FormaPagamento = despesa.FormaPagamento,
                                Valor = despesa.Valor,
                                Parcelas = despesa.Parcelas,
                                Observacao = despesa.Observacao
                            }).ToListAsync();


        if (despesas is null || despesas.Count == 0)
        {
            return NotFound("Não foram encontrados despesas para o usuário especificado.");
        }

        return Ok(despesas);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] Despesas despesa)
    {
        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                dbContext.Despesas.Add(despesa);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { OrderId = despesa.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Ocorreu um erro ao processar a despesa.");
            }
        }
    }
}
