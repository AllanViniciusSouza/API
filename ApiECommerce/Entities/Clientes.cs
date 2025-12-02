using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Clientes
{
    public int Id { get; set; }  // Campo Auto Increment no banco de dados
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    [StringLength(150)]
    public string? Email { get; set; } = string.Empty;
    [StringLength(80)]
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    //public DateTime? DataNascimento { get; set; }  // Usando DateTime? para permitir valores nulos
    [JsonIgnore]
    public ICollection<Pedido>? Pedidos { get; set; }
    [JsonIgnore]
    public Comanda? Comanda { get; set; }
    [JsonIgnore]
    public CascosEmprestados? CascosEmprestados { get; set; }

}


