using ApiECommerce.Context;
using ApiECommerce.DTOs;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ComandasController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public ComandasController(AppDbContext context)
    {
        dbContext = context;
    }

    // Criar uma nova comanda (POST /api/comandas)
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ComandasDTO dto)
    {

        if (dto == null)
        {
            return BadRequest("Dados inválidos.");
        }

        var novaComanda = new Comanda
        {
            Nome = dto.Nome,
            Telefone = dto.Telefone,
            Endereco = dto.Endereco,
            DataAbertura = DateTime.Now,
            FormaPagamento = dto.FormaPagamento,
            ValorRecebido = dto.ValorRecebido ?? 0,
            ClienteId = dto.ClienteId,
            UsuarioId = dto.UsuarioId,
            Status = dto.Status,
            ValorTotal = dto.Itens.Sum(i => i.PrecoUnitario * i.Quantidade),
            Itens = dto.Itens?.Select(i => new ItemComanda
            {
                Id = i.Id,
                Nome = i.Nome,
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario
            }).ToList() ?? new List<ItemComanda>()
        };

        using ( var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                dbContext.Comandas.Add(novaComanda);

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(Post), new { id = novaComanda.Id });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Ocorreu um erro ao processar a comanda.");
            }
        }
    }

    // GET comandas (GET /api/comandas)
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Comandas()
    {
        var comandas = await dbContext.Comandas
                        .Where(c => c.Status != "Fechada")
                        .Include(c => c.Itens) // Carrega os Itens da Comanda junto
                                               .ThenInclude(i => i.Produto)
                       .OrderByDescending(comanda => comanda.DataAbertura)
                       .ToListAsync();


        // Verifica se há comandas, caso contrário, retorna NotFound
        if (comandas == null || comandas.Count == 0)
        {
            return NotFound("Nenhuma comanda encontrada.");
        }

        // 🔁 Mapeia Comanda → ComandasDTO
        var resultado = comandas.Select(c => new ComandasDTO
        {
            Id = c.Id,
            Nome = c.Nome,
            Telefone = c.Telefone,
            DataAbertura = c.DataAbertura,
            Status = c.Status,
            Endereco = c.Endereco,
            ValorTotal = c.ValorTotal,
            FormaPagamento = c.FormaPagamento,
            ValorRecebido = c.ValorRecebido,
            ClienteId = c.ClienteId,
            UsuarioId = c.UsuarioId,
            //Itens = c.Itens?.Select(i => new ItemComanda
            //{
            //    Id = i.Id,
            //    Nome = i.Nome,
            //    ProdutoId = i.ProdutoId,
            //    Quantidade = i.Quantidade,
            //    PrecoUnitario = i.PrecoUnitario
            //}).ToList() ?? new List<ItemComanda>()
        }).ToList();

        return Ok(resultado);
    }

    // GET comandas (GET /api/comandas)
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ComandasAbertas()
    {
        var comandas = await dbContext.Comandas
                       .OrderByDescending(comanda => comanda.DataAbertura)
                       .Select(comanda => new
                       {
                           Id = comanda.Id,
                           IdComanda = comanda.Id,
                           ValorTotal = comanda.ValorTotal,
                           DataAbertura = comanda.DataAbertura
                       })
                       .ToListAsync();


        // Verifica se há comandas, caso contrário, retorna NotFound
        if (comandas == null || comandas.Count == 0)
        {
            return NotFound("Nenhuma comanda encontrada.");
        }

        return Ok(comandas); // Retorna todas as comandas
    }

    // GET comandas (GET /api/comandas)
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ComandasFechadas()
    {
        var comandas = await dbContext.DetalhesComanda
                             .OrderByDescending(dc => dc.DataAbertura)
                             .Select(comanda => new
                              {
                                  ComandaId = comanda.Id,
                                  ValorTotal = comanda.ValorTotal,
                                  DataAbertura = comanda.DataAbertura,
                                  DataPedido = comanda.DataPedido,
                                  FormaPagamento = comanda.FormaPagamento
                                  //IdComanda = comanda.IdComanda
                              })
                            .ToListAsync();



        // Verifica se há comandas, caso contrário, retorna NotFound
        if (comandas == null || comandas.Count == 0)
        {
            return NotFound("Nenhuma comanda encontrada.");
        }

        return Ok(comandas); // Retorna todas as comandas
    }


    // 2️⃣ Obter detalhes de uma comanda específica (GET /api/comandas/{id})
    [HttpGet("[action]/{IdComanda}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ComandaPorId(int Id)
    {
        var comandaDetalhes = await dbContext.Comandas
                                    .FirstOrDefaultAsync(d => d.Id == Id);

        if (comandaDetalhes == null)
        {
            return NotFound("Comanda não encontrada.");
        }

        return Ok(comandaDetalhes);
    }

    [HttpGet("[action]/{Nome}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ComandaPorNome(string Nome)
    {
        var comandaDetalhes = await dbContext.Comandas
                                    .FirstOrDefaultAsync(d => d.Nome == Nome);

        if (comandaDetalhes == null)
        {
            return NotFound("Comanda não encontrada.");
        }

        return Ok(comandaDetalhes);
    }

    // 3️⃣ Listar todas as comandas de um usuário (GET /api/comandas/usuario/{nomeCliente})
    [HttpGet("[action]/{usuarioId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ComandasPorUsuario(int usuarioId)
    {
        var comandas = await dbContext.Comandas
            .Where(c => c.UsuarioId == usuarioId)
            .OrderByDescending(c => c.DataAbertura)
            .Select(c => new
            {
                Id = c.Id,
                Status = c.Status,
                PedidoTotal = c.ValorTotal,
                DataPedido = c.DataAbertura
            }).ToListAsync();


        if (comandas.Count == 0)
        {
            return NotFound("Nenhuma comanda encontrada para este usuário.");
        }

        return Ok(comandas);
    }

    [HttpPut("{nome}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarComanda(string nome, [FromBody] ComandasDTO comandaAtualizada)
    {
        if (nome != comandaAtualizada.Nome)
        {
            return BadRequest("ID da comanda inválido.");
        }

        var comandaExistente = await dbContext.Comandas.FirstOrDefaultAsync(c => c.Nome == nome);

        if (comandaExistente == null)
        {
            return NotFound("Comanda não encontrada.");
        }

        comandaExistente.DataAbertura = comandaAtualizada.DataAbertura;
        comandaExistente.UsuarioId = comandaAtualizada.UsuarioId;
        comandaExistente.ClienteId = comandaAtualizada.ClienteId;
        comandaExistente.ValorTotal = comandaAtualizada.ValorTotal;
        comandaExistente.ValorRecebido = comandaAtualizada.ValorRecebido;
        comandaExistente.FormaPagamento = comandaAtualizada.FormaPagamento;

        dbContext.Comandas.Update(comandaExistente);
        await dbContext.SaveChangesAsync();

        return Ok(comandaExistente);
    }

    // DELETE: api/comandas/{idComanda}
    [HttpDelete("{nome}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComanda(string nome)
    {
        // Busca a comanda pelo IdComanda
        var comanda = await dbContext.Comandas
                                     .FirstOrDefaultAsync(c => c.Nome == nome);

        // Verifica se a comanda existe
        if (comanda == null)
        {
            return NotFound("Comanda não encontrada.");
        }

        // Remove a comanda do contexto
        dbContext.Comandas.Remove(comanda);

        // Salva as mudanças no banco de dados
        await dbContext.SaveChangesAsync();

        // Retorna NoContent para indicar sucesso sem conteúdo
        return NoContent();
    }

}
