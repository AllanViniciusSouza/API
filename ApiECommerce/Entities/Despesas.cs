using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities
{
    public class Despesas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }

        public string FormaPagamento { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        public int Parcelas { get; set; }

        public string? Observacao { get; set; }

        // Relacionamento: Uma despesa tem um usuário
        public int UsuarioId { get; set; } 
    }
}
