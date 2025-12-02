using ApiECommerce.Entities;
using System.Text.Json.Serialization;

namespace ApiECommerce.DTOs;

public class ComandasCardapioDTO
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public DateTime DataAbertura { get; set; }
    public int? ClienteId { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? ValorRecebido { get; set; }
    public string? FormaPagamento { get; set; }
    public List<ItemComandaDTO>? Itens { get; set; }
}
