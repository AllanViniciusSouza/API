using ApiECommerce.Context;
using ApiECommerce.DTOs;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public PedidosController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET: api/Pedidos/DetalhesPedido/5
    // Retorna os detalhes de um pedido específico, incluindo informações sobre
    // os produtos associados a esse pedido.
    //[HttpGet("[action]/{pedidoId}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> DetalhesPedido(int pedidoId)
    //{
    //    var pedidoDetalhes = await dbContext.DetalhesPedido
    //                                .Where(d => d.PedidoId == pedidoId)
    //                                .Select(detalhePedido => new
    //                                {
    //                                    Id = detalhePedido.Id,
    //                                    Quantidade = detalhePedido.Quantidade,
    //                                    SubTotal = detalhePedido.ValorTotal,
    //                                    FormaPagamento = detalhePedido.FormaPagamento
    //                                }).ToListAsync();

    //    if (!pedidoDetalhes.Any())
    //    {
    //        return NotFound("Detalhes do pedido não encontrados.");
    //    }

    //    return Ok(pedidoDetalhes);
    //}


    //// GET: api/Pedidos/PedidosPorUsuario/5
    //// Obtêm todos os pedidos de um usuário específico com base no ID do usuário.
    //[HttpGet("[action]/{usuarioId}")]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> PedidosPorUsuario(int usuarioId)
    //{
    //    var pedidos = await dbContext.Pedidos
    //                        .Where(pedido => pedido.ClienteId == usuarioId)
    //                        .OrderByDescending(pedido => pedido.DataPedido)
    //                        .Select(pedido => new
    //                        {
    //                            Id = pedido.Id,
    //                            NomeCliente = pedido.Cliente.Nome,
    //                            PedidoTotal = pedido.ValorTotal,
    //                            DataPedido = pedido.DataPedido
    //                        }).ToListAsync();


    //    if (pedidos is null || pedidos.Count == 0)
    //    {
    //        return NotFound("Não foram encontrados pedidos para o usuário especificado.");
    //    }

    //    return Ok(pedidos);
    //}

    //// GET: api/Pedidos/TodosPedidos/5
    //// Obtêm todos os pedidos 
    [HttpGet("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PedidosHoje()
    {
        var pedidos = await dbContext.Pedidos
                            .Include(p => p.Usuario)
                            .Where(pedido => pedido.DataPedido.HasValue && pedido.DataPedido.Value.Date == DateTime.Today)
                            .OrderByDescending(pedido => pedido.DataPedido)
                            .Select(pedido => new ApiECommerce.DTOs.PedidoDTO
                            {
                                Id = pedido.Id,
                                ValorTotal = pedido.ValorTotal,
                                FormaPagamento = pedido.FormaPagamento,
                                DataPedido = pedido.DataPedido,
                                ValorPago1 = pedido.ValorTotal, // caso não tenha campos separados
                                FormaPagamento2 = null,
                                ValorPago2 = null,
                                Status = string.Empty,
                                ClienteNome = pedido.ClienteNome != null ? pedido.ClienteNome : null,
                                VendedorNome = pedido.Usuario != null ? pedido.Usuario.Nome : null,
                                DataPagamentoPrazo = pedido.DataPagamentoPrazo,
                                DataPagamentoPrazo2 = pedido.DataPagamentoPrazo2,
                                Observacoes = pedido.Observacoes
                            }).ToListAsync();


        if (pedidos is null || pedidos.Count == 0)
        {
            return NotFound("Não foram encontrados pedidos para o usuário especificado.");
        }

        return Ok(pedidos);
    }

    // GET: api/Pedidos/Detalhes/{pedidoId}
    [HttpGet("Detalhes/{pedidoId}")]
    public async Task<IActionResult> GetPedidoDetalhes(int pedidoId)
    {
        if (pedidoId <= 0)
            return BadRequest("pedidoId inválido.");

        try
        {
            var detalhes = await dbContext.DetalhesPedido
                .Where(d => d.PedidoId == pedidoId)
                .Include(d => d.Produto)
                .Select(d => new ApiECommerce.DTOs.PedidoDetalheDTO
                {
                    Id = d.Id,
                    ProdutoId = d.ProdutoId,
                    ProdutoNome = d.Produto != null ? d.Produto.Nome : d.ProdutoNome,
                    ProdutoImagem = d.Produto != null ? d.Produto.UrlImagem : null,
                    Quantidade = d.Quantidade,
                    ProdutoPreco = d.Preco,
                    SubTotal = d.ValorTotal,
                    CaminhoImagem = d.Produto != null ? d.Produto.UrlImagem : null
                }).ToListAsync();

            // sempre retornar 200 OK com lista (mesmo vazia) para simplificar tratamento no frontend
            return Ok(detalhes ?? new List<ApiECommerce.DTOs.PedidoDetalheDTO>());
        }
        catch (Exception ex)
        {
            // log simples para ajudar debug local; ideal usar ILogger em produção
            Console.WriteLine($"Erro ao buscar detalhes do pedido {pedidoId}: {ex}");
            return StatusCode(500, "Erro ao buscar detalhes do pedido.");
        }
    }

    // GET: api/pedidos/data/2025-04-22
    [HttpGet("data/{data:datetime}")]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidosPorData(DateTime data)
    {
        // Pega só o dia, ignorando hora
        var pedidos = await dbContext.Pedidos
            .Include(p => p.Usuario)
            .Where(p => p.DataPedido.HasValue && p.DataPedido.Value.Date == data.Date)
            .OrderByDescending(p => p.DataPedido)
            .Select(p => new ApiECommerce.DTOs.PedidoDTO
            {
                Id = p.Id,
                Endereco = p.Endereco,
                ValorTotal = p.ValorTotal,
                DataPedido = p.DataPedido,
                FormaPagamento = p.FormaPagamento,
                ValorPago1 = p.ValorTotal,
                FormaPagamento2 = p.FormaPagamento2,
                ValorPago2 = p.ValorPago2,
                Status = p.Status,
                ClienteNome = p.ClienteNome != null ? p.ClienteNome : null,
                VendedorNome = p.VendedorNome != null ? p.VendedorNome : null
            }).ToListAsync();

        return Ok(pedidos);
    }

    //---------------------------------------------------------------------------
    // Neste codigo a criação do pedido, a adição dos detalhes do pedido
    // e a remoção dos itens do carrinho são agrupados dentro de uma transação única.
    // Se alguma operação falhar, a transação será revertida e nenhuma alteração será
    // persistida no banco de dados. Isso garante a consistência dos dados e evita a
    // possibilidade de criar um pedido sem itens no carrinho ou deixar itens
    // no carrinho após criar o pedido.
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] PedidoDTO pedido)
    {
        // normalizar campos vindos do front (aceita tanto ProdutoId/ProdutoPreco/SubTotal quanto Produto/PrecoUnitario/Total)
        var normalizedItems = (pedido.Itens ?? new List<PedidoDetalheDTO>())
            .Select(i => new {
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.ProdutoNome,
                Preco = i.ProdutoPreco,
                Quantidade = i.Quantidade,
                SubTotal = i.SubTotal
            }).ToList();

        // validar ids de produto (apenas >0)
        var productIds = normalizedItems
            .Select(x => x.ProdutoId)
            .Where(id => id.HasValue && id.Value > 0)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        var existing = await dbContext.Produtos.Where(p => productIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
        var missing = productIds.Except(existing).ToList();
        if (missing.Any())
            return BadRequest($"Produtos não encontrados: {string.Join(',', missing)}");

        // construir pedido com itens normalizados (somente uma vez)
        var newPedido = new Pedido {
            Endereco = pedido.Endereco,
            ValorTotal = pedido.ValorTotal,
            DataPedido = DateTime.Now,
            FormaPagamento = pedido.FormaPagamento,
            ValorPago1 = pedido.ValorPago1,
            FormaPagamento2 = pedido.FormaPagamento2,
            ValorPago2 = pedido.ValorPago2,
            Status = pedido.Status,
            ClienteNome = pedido.ClienteNome,
            VendedorNome = pedido.VendedorNome,
            DataPagamentoPrazo = pedido.DataPagamentoPrazo,
            DataPagamentoPrazo2 = pedido.DataPagamentoPrazo2,
            Observacoes = pedido.Observacoes,
            Itens = normalizedItems.Select(x => new DetalhePedido {
                ProdutoId = (x.ProdutoId.HasValue && x.ProdutoId.Value > 0) ? x.ProdutoId.Value : (int?)null,
                ProdutoNome = string.IsNullOrWhiteSpace(x.ProdutoNome) ? null : x.ProdutoNome,
                Preco = x.Preco,
                Quantidade = x.Quantidade,
                ValorTotal = x.SubTotal,
                FormaPagamento = pedido.FormaPagamento
            }).ToList()
        };

        dbContext.Pedidos.Add(newPedido);
        await dbContext.SaveChangesAsync();
        return Ok(new { OrderId = newPedido.Id });
    }

    [HttpPost("[action]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostComanda([FromBody] ComandasDTO dto)
    {
       var itensComanda = await dbContext.ItensComanda
            .Where(ic => ic.Nome == dto.Nome)
            .ToListAsync();

        var comanda = await dbContext.Comandas
            .FirstOrDefaultAsync(d => d.Id == dto.Id);

        // Verifica se há itens no carrinho
        if (itensComanda.Count == 0)
        {
            return NotFound("Não há itens no carrinho para criar o pedido.");
        }

        using (var transaction = await dbContext.Database.BeginTransactionAsync())
        {
            try
            {
                var detalhesComanda = itensComanda.Select(item => new DetalhesComanda
                {
                    Preco = item.PrecoUnitario,
                    ValorTotal = item.PrecoUnitario * item.Quantidade,
                    Quantidade = item.Quantidade,
                    ProdutoId = item.ProdutoId,
                    //IdComanda = comanda.IdComanda,
                    FormaPagamento = dto.FormaPagamento,
                    DataPedido = DateTime.Now
                }).ToList();

                var pedido = new Pedido()
                {
                    ValorTotal = dto.ValorTotal,
                    DataPedido = DateTime.Now,
                    FormaPagamento = dto.FormaPagamento
                };

                dbContext.Pedidos.Add(pedido);
                dbContext.DetalhesComanda.AddRange(detalhesComanda);
                dbContext.ItensComanda.RemoveRange(itensComanda);
                dbContext.Comandas.Remove(comanda);

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return BadRequest("Ocorreu um erro ao processar o pedido.");
            }
        }
    }
}
