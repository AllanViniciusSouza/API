using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CascosEmprestadosController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public CascosEmprestadosController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: Pega a lista de clientes com cascos emprestados
    [HttpGet("[action]")]
    public async Task<IActionResult> TodosRegistros()
    {
        var registrosCascosEmprestados = await dbContext.CascosEmprestados
                                                .Where(ce => ce.Status == "Emprestado")
                                                .OrderByDescending(ce => ce.DataEmprestimo)
                                                .Select(ce => new
                                                {
                                                    Id = ce.Id,
                                                    NomeCliente = ce.Cliente.Nome,
                                                    Telefone = ce.Cliente.Telefone,
                                                    Quantidade = ce.Quantidade,
                                                    //Produto = ce.Produto,
                                                    DataEmprestimo = ce.DataEmprestimo,
                                                    DataDevolucao = ce.DataDevolucao
                                                }).ToListAsync();

        if (registrosCascosEmprestados is null || registrosCascosEmprestados.Count == 0)
        {
            return NotFound("Não foram encontrados registros de empréstimos de cascos.");
        }

        return Ok(registrosCascosEmprestados);
    }

    // GET: Pega os detalhes de um registro específico
    [HttpGet("[action]/{registroid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DetalhesRegistro(int registroid)
    {
        var registroDetalhes = await dbContext.DetalhesCascosEmprestados
                                    .Where(dce => dce.Id == registroid)
                                    .Select(detalheRegistro => new
                                    {
                                        Id = detalheRegistro.Id,
                                        Quantidade = detalheRegistro.Quantidade,
                                        NomeProduto = detalheRegistro.Produto.Nome
                                    }).ToListAsync();

        if (!registroDetalhes.Any())
        {
            return NotFound("Detalhes do pedido não encontrados.");
        }

        return Ok(registroDetalhes);
    }

    // GET: CascosEmprestadosController/Create
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CascosEmprestados registro)
    {
        if (registro == null)
        {
            return BadRequest("Dados inválidos.");
        }

        registro.DataEmprestimo = DateTime.Now;

        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                dbContext.CascosEmprestados.Add(registro);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(Create), new { id = registro.Id }, registro);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Ocorreu um erro ao processar a comanda.");
            }
        }
    }

    // DELETE: CascosEmprestadosController/Delete/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest("ID inválido.");
        }

        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var registro = await dbContext.CascosEmprestados.FindAsync(id);

                if (registro == null)
                {
                    return NotFound("Registro não encontrado.");
                }

                dbContext.CascosEmprestados.Remove(registro);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok("Registro excluído com sucesso.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Erro ao excluir o registro.");
            }
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarRegistro(int id, [FromBody] CascosEmprestados registroAtualizada)
    {
        if (id != registroAtualizada.Id)
        {
            return BadRequest("ID do registro inválido.");
        }

        var registroExistente = await dbContext.CascosEmprestados.FindAsync(id);

        if (registroExistente == null)
        {
            return NotFound("Registro não encontrada.");
        }

        // Atualiza o status do registro no banco
        registroExistente.Status = "Devolvido";

        dbContext.CascosEmprestados.Update(registroExistente);
        await dbContext.SaveChangesAsync();

        return Ok(registroExistente);
    }

}
