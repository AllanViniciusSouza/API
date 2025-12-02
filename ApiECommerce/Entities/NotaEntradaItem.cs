using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System;

namespace ApiECommerce.Entities;

public class NotaEntradaItem
{
    public int Id { get; set; }
    public int NotaEntradaId { get; set; }
    [JsonIgnore]
    public NotaEntrada? NotaEntrada { get; set; }

    public int? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    public string? Nome { get; set; }
    public string? Barcode { get; set; }
    public int Quantidade { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? PrecoCusto { get; set; }
}
