using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Produto
{
    public int Id { get; set; }
    [StringLength(100)]
    [Required]
    public string? Nome { get; set; }
    [StringLength(20)]
    public string? Barcode { get; set; }
    [StringLength(200)]
    public string? Detalhe { get; set; }
    [StringLength(200)]
    public string? UrlImagem { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Preco { get; set; }
       public bool Popular { get; set; }
    public bool MaisVendido { get; set; }
    public int EmEstoque { get; set; }
    public string? DiasDisponiveis { get; set; }
    public bool Disponivel { get; set; }
    public int CategoriaId { get; set; }


    [JsonIgnore]
    public ICollection<ItemCarrinhoCompra>? ItensCarrinhoCompras { get; set; }
    [JsonIgnore]
    public ICollection<DetalhesComanda>? DetalhesComanda { get; set; }
    [JsonIgnore]
    public ICollection<ItemComanda>? ItensComanda { get; set; }
    [JsonIgnore]
    public Estoque? Estoque { get; set; }
    [JsonIgnore]
    public ICollection<MovimentacaoEstoque>? Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
    [JsonIgnore]
    public ICollection<Inventario>? Inventarios { get; set; } = new List<Inventario>();

    public void AtualizarDisponibilidade()
    {
        if (string.IsNullOrWhiteSpace(DiasDisponiveis))
        {
            Disponivel = false;
            return;
        }

        var dias = DiasDisponiveis
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .ToList();

        var hoje = DateTime.Today.DayOfWeek.ToString();
        Disponivel = dias.Any(d => string.Equals(d, hoje, StringComparison.OrdinalIgnoreCase));
    }
}
