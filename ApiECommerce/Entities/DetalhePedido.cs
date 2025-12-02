using System.ComponentModel.DataAnnotations.Schema;

namespace ApiECommerce.Entities;

public class DetalhePedido
{
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }
    
    public int Quantidade { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotal { get; set; }
    public string FormaPagamento { get; set; }
    public int PedidoId { get; set; }
    public Pedido Pedido { get; set; }
    // relacionamento com o produto pedido
    public int ProdutoId { get; set; }
    public Produto? Produto { get; set; }
}
