using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class MovimentacaoEstoque
{
    public int Id { get; set; } // Chave primária
    public int ProdutoId { get; set; } // Chave estrangeira para Produto
   // public int EstoqueId { get; set; }
    public int Quantidade { get; set; } // Quantidade movimentada
    [Column(TypeName = "decimal(10,2)")]
    public decimal? PrecoCusto { get; set; }
    public TipoMovimentacao Tipo { get; set; } // Entrada ou Saída
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow; // Data do movimento
    public string? Observacao { get; set; } // Comentário opcional sobre a movimentação
    [JsonIgnore]
    // Relacionamento com Produto
    public Produto? Produto { get; set; }
    //[JsonIgnore]
    //public Estoque Estoque { get; set; }
}
