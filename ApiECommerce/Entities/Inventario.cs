namespace ApiECommerce.Entities
{
    public class Inventario
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int QuantidadeFisica { get; set; } // Contagem real feita pelo responsável
        public DateTime DataInventario { get; set; } = DateTime.UtcNow;
        public string UsuarioResponsavel { get; set; }

        public Produto Produto { get; set; }
    }
}
