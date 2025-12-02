using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public ClientesController(AppDbContext dbcontext)
    {
        dbContext = dbcontext;
    }

    // GET: api/clientes - Retorna todos os clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Clientes>>> GetClientes()
    {
        return await dbContext.Clientes.ToListAsync();
    }

    // GET: api/clientes/5 - Retorna um cliente pelo ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Clientes>> GetCliente(int id)
    {
        var cliente = await dbContext.Clientes.FindAsync(id);

        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        return cliente;
    }

    // GET: api/clientes/5 - Retorna um cliente pelo Nome
    [HttpGet("byname/{nome}")]
    public async Task<ActionResult<Clientes>> GetClienteByName(string nome)
    {
        var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Nome == nome);

        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        return cliente;
    }

    // GET: api/clientes/5 - Retorna um cliente pelo Telefone
    [HttpGet("byphone/{telefone}")]
    public async Task<ActionResult<Clientes>> GetClienteByPhone(string telefone)
    {
        var cliente = await dbContext.Clientes.FirstOrDefaultAsync(c => c.Telefone == telefone);

        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        return cliente;
    }

    // POST: api/clientes - Adiciona um novo cliente
    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Clientes cliente)
    {
        var clienteExiste = await dbContext.Clientes.FirstOrDefaultAsync(u => u.Nome == cliente.Nome);

        if (clienteExiste is not null)
        {
            return BadRequest("Já existe cliente com este nome");
        }

        dbContext.Clientes.Add(cliente);
        await dbContext.SaveChangesAsync();
        // Retorna o cliente com o ID gerado
        return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
    }

    // PUT: api/clientes/5 - Atualiza um cliente existente
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCliente(int id, Clientes cliente)
    {
        if (id != cliente.Id)
        {
            return BadRequest(new { message = "IDs não coincidem" });
        }

        dbContext.Entry(cliente).State = EntityState.Modified;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClienteExists(id))
            {
                return NotFound(new { message = "Cliente não encontrado" });
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/clientes/5 - Remove um cliente pelo ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        var cliente = await dbContext.Clientes.FindAsync(id);
        if (cliente == null)
        {
            return NotFound(new { message = "Cliente não encontrado" });
        }

        dbContext.Clientes.Remove(cliente);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    // Método auxiliar para verificar se o cliente existe
    private bool ClienteExists(int id)
    {
        return dbContext.Clientes.Any(e => e.Id == id);
    }
}
