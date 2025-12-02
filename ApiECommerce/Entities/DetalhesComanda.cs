using System.ComponentModel.DataAnnotations.Schema;

namespace ApiECommerce.Entities;
public class DetalhesComanda
{
    public int Id { get; set; } // Chave primária
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; } // Preço unitário do produto
    public int Quantidade { get; set; } // Quantidade do produto
    [Column(TypeName = "decimal(12,2)")]
     public decimal ValorTotal { get; set; }
    public string? FormaPagamento { get; set; }
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime DataPedido { get; set; }
}
