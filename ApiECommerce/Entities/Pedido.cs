using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Pedido
{
    public int Id { get; set; }
    [StringLength(100)]
    public string? Endereco { get; set; }

    [Column(TypeName = "decimal(12,2)")]
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

    // Relacionamento: Um Pedido tem um usuário (vendedor)
    [JsonIgnore]
    public int? UsuarioId { get; set; }
    [JsonIgnore]
    public Usuario? Usuario { get; set; }

    // Relacionamento: Um Pedido tem vários detalhes (itens) do pedido
    [JsonIgnore]
    public ICollection<DetalhePedido>? Itens { get; set; } = new List<DetalhePedido>();

}
