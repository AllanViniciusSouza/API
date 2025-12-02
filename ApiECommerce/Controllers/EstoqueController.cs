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
}
