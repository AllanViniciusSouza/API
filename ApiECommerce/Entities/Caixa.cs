namespace ApiECommerce.Entities;

public class Caixa
{
    public int Id { get; set; }

    // Quem abriu o caixa
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }  // relacionamento

    public DateTime DataAbertura { get; set; }
    public decimal ValorAbertura { get; set; }

    public DateTime? DataFechamento { get; set; }  // nullable: só fecha depois
    public decimal? ValorFechamento { get; set; }  // nullable: idem

    public string? Observacao { get; set; }

    public bool CaixaFechado => DataFechamento.HasValue;
}
