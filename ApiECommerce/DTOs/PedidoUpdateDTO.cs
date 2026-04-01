using System;

namespace ApiECommerce.DTOs;

public class PedidoUpdateDTO
{
    public string? Endereco { get; set; }
    public string? FormaPagamento { get; set; }
    public string? FormaPagamento2 { get; set; }
    public string? Status { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime? DataPagamentoPrazo { get; set; }
    public DateTime? DataPagamentoPrazo2 { get; set; }
    public string? Observacoes { get; set; }
    // Itens que podem ser atualizados: Id refere-se ao DetalhePedido.Id
    public List<PedidoItemUpdateDTO>? Itens { get; set; }
}

public class PedidoItemUpdateDTO
{
    public int Id { get; set; }
    // novo preço unitário (opcional)
    public decimal? Preco { get; set; }
    // nova quantidade (opcional)
    public int? Quantidade { get; set; }
}

