using ApiECommerce.Context;
using ApiECommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpressaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImpressaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetComandasParaImpressao()
        {
            var comandas = await _context.Comandas
                                    .Where(c => c.Status == "Aguardando Impressao")
                                    .OrderBy(c => c.Id)
                                    .Select(c => new
                                    {
                                        Comanda = c,
                                        Itens = _context.ItensComanda
                                            .Where(ic => ic.Nome == c.Nome)
                                            .Include(ic => ic.Produto)
                                            .ToList()
                                    })
                                    .ToListAsync();


            if (!comandas.Any())
                return Ok(new List<ComandaImpressaoDTO>());

            var lista = comandas.Select(c =>
            {
                var itensTexto = string.Join("\n", c.Itens
                     //.Where(i => i.Produto.CategoriaId == 99) // filtro direto
                    .Select(i => $"-{i.Quantidade}x{i.Produto?.Nome ?? "Produto"}"));

                //var itensTexto = string.Join("\n", c.Itens.Select(i =>
           //$"- {i.Quantidade} x {i.Produto?.Nome ?? "Produto"} R${i.PrecoUnitario:0.00}"));

                return new ComandaImpressaoDTO
                {
                    Id = c.Comanda.Id,
                    Texto = $"Comanda #{c.Comanda.Id}\nCliente: {c.Comanda.Nome}\n{itensTexto}\n\nEndereco: {c.Comanda.Endereco}\nTotal: R${c.Comanda.ValorTotal:0.00}"
                };
            });


            return Ok(lista);
        }

        // PUT: Atualiza status das comandas para "Impresso"
        [HttpPut]
        public async Task<IActionResult> AtualizarStatusComandas([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Nenhuma comanda para atualizar.");

            var comandas = await _context.Comandas
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!comandas.Any())
                return NotFound("Nenhuma comanda encontrada com os IDs informados.");

            foreach (var comanda in comandas)
            {
                comanda.Status = "Impresso";
            }

            await _context.SaveChangesAsync();

            return Ok("Status atualizado.");
        }
    }
}
