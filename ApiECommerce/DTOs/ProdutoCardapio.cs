namespace ApiECommerce.DTOs;

public class ProdutoCardapio
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public decimal PrecoQuente { get; set; }
    public decimal PrecoGelada { get; set; }
    public decimal PrecoEntrega { get; set; }
    public decimal PrecoRetirar { get; set; }
}
