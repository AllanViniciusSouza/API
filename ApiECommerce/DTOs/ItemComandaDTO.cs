using System.Text.Json.Serialization;

namespace ApiECommerce.DTOs;

public class ItemComandaDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
