using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Estoque
{
    public int Id { get; set; } // Identificador único do estoque
    public int ProdutoId { get; set; } // Referência ao produto
    public Produto? Produto { get; set; } // Navegação para a entidade Produto
    public int Quantidade { get; set; } // Quantidade disponível no estoque
    public int? QuantidadeMinima { get; set; }
    public DateTime DataEntrada { get; set; } // Data de entrada do produto no estoque
    public DateTime? DataValidade { get; set; } // Data de validade (se aplicável)
    [Column(TypeName = "decimal(10,2)")]
    public decimal? PrecoCusto { get; set; } // Preço de custo do produto no estoque
    public int? Margem { get; set; }

    [NotMapped]
    public decimal? PrecoVenda
    {
        get
        {
            if (PrecoCusto.HasValue && Margem.HasValue)
            {
                return PrecoCusto.Value + (PrecoCusto.Value * Margem.Value / 100);
            }
            return null;
        }
    }

    public string? LocalArmazenamento { get; set; } // Local onde o produto está armazenado
    public bool Ativo { get; set; } = true; // Indica se o item está ativo no estoque
    [JsonIgnore]
    public ICollection<MovimentacaoEstoque>Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();

}
