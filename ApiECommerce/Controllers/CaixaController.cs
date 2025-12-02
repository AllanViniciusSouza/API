using ApiECommerce.Context;
using ApiECommerce.Entities;
using ApiECommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CaixaController : ControllerBase
{
    private readonly AppDbContext _context;

    public CaixaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/caixas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Caixa>>> GetCaixas()
    {
        return await _context.Caixas.Include(c => c.Usuario).ToListAsync();
    }

    // GET: api/caixas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Caixa>> GetCaixa(int id)
    {
        var caixa = await _context.Caixas.Include(c => c.Usuario)
                                         .FirstOrDefaultAsync(c => c.Id == id);

        if (caixa == null)
            return NotFound();

        return caixa;
    }

    // POST: api/caixas/abrir
    [HttpPost("abrir")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AbrirCaixa([FromBody] AbrirCaixaDto dto)
    {
        var caixaAberto = await _context.Caixas
            .FirstOrDefaultAsync(c => c.DataFechamento == null);

        if (caixaAberto != null)
            return BadRequest("Já existe um caixa aberto para este usuário.");

        var caixa = new Caixa
        {
            UsuarioId = 1,
            DataAbertura = DateTime.Now,
            ValorAbertura = dto.ValorAbertura,
            Observacao = dto.Observacao
        };

        _context.Caixas.Add(caixa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCaixa), new { id = caixa.Id }, caixa);
    }

    // PUT: api/caixas/fechar/5
    [HttpPut("fechar/{id}")]
    public async Task<IActionResult> FecharCaixa(int id, [FromBody] FecharCaixaDto dto)
    {
        var caixa = await _context.Caixas.FindAsync(id);

        if (caixa == null)
            return NotFound();

        if (caixa.DataFechamento != null)
            return BadRequest("Caixa já foi fechado.");

        caixa.DataFechamento = DateTime.Now;
        caixa.ValorFechamento = dto.ValorFechamento;
        caixa.Observacao += "\n" + dto.Observacao;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

