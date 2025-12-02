using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class ItemComanda
{
    public int Id { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }
    public string Nome { get; set; }
    public int ProdutoId { get; set; }
    public Produto Produto { get; set; } // <-- ESSA PROPRIEDADE PRECISA EXISTIR
    public int? ComandaId { get; set; }
    public Comanda? Comanda { get; set; }

}
