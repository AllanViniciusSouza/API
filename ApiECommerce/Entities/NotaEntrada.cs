using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System;

namespace ApiECommerce.Entities;

public class NotaEntrada
{
    public int Id { get; set; }
    public string? NumeroNota { get; set; }
    public DateTime DataEmissao { get; set; } = DateTime.UtcNow;
    public string? Fornecedor { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ValorTotal { get; set; }

    [JsonIgnore]
    public ICollection<NotaEntradaItem> Itens { get; set; } = new List<NotaEntradaItem>();
}
