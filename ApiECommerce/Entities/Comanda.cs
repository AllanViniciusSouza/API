using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities;

public class Comanda
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public DateTime DataAbertura { get; set; } = DateTime.Now; // Padrão: data atual   
    public string Status { get; set; }
    [Column(TypeName = "decimal(12,2)")]
    public decimal ValorTotal { get; set; }
    public string? FormaPagamento { get; set; }
    [Column(TypeName = "decimal(12,2)")]
    public decimal? ValorRecebido { get; set; }

    // Relacionamento: Uma comanda tem vários itens
    public List<ItemComanda>? Itens { get; set; } = new List<ItemComanda>();

    //Relacionameno: Uma comanda tem apenas um cliente
    public int? ClienteId { get; set; }
    public Clientes? Cliente { get; set; }

    // Relacionamento: Uma comanda tem um usuário
    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    //public void AtualizarValorTotal()
    //{
    //    ValorTotal = Itens?.Sum(i => i.Quantidade * i.PrecoUnitario) ?? 0;
    //}
}

