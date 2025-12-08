using ApiECommerce.Entities;

namespace ApiECommerce.DTOs;

public class PedidoDTO
{
    public int Id { get; set; }
    public string? Endereco { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime? DataPedido { get; set; }
    public string? FormaPagamento { get; set; }

    // Primary payment
    public decimal? ValorPago1 { get; set; }

    // Secondary payment
    public string? FormaPagamento2 { get; set; }
    public decimal? ValorPago2 { get; set; }

    public string? Status { get; set; }

    // New fields requested by front-end
    public string? ClienteNome { get; set; }
    public string? VendedorNome { get; set; }
    public DateTime? DataPagamentoPrazo { get; set; }
    public DateTime? DataPagamentoPrazo2 { get; set; }

    // Observações livres do pedido
    public string? Observacoes { get; set; }

    // Item details
    public List<PedidoDetalheDTO>? Itens { get; set; }

    public int? UsuarioId { get; set; }
}

public class PedidoDetalheDTO
{
    public int Id { get; set; }
    public int? ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public string? ProdutoImagem { get; set; }
    public int Quantidade { get; set; }
    public decimal ProdutoPreco { get; set; }
    public decimal SubTotal { get; set; }
    public string? CaminhoImagem { get; set; }
}
