using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Categoria
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string? Nome { get; set; }
    [StringLength(200)]
    public string? UrlImagem { get; set; }

    public ICollection<Produto>? Produtos { get; set; }
}
