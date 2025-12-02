using ApiECommerce.Entities;

namespace ApiECommerce.Repositories
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> ObterProdutosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Produto>> ObterProdutosPopularesAsync();
        Task<IEnumerable<Produto>> ObterProdutosMaisVendidosAsync();
        Task<IEnumerable<Produto>> ObterTodosProdutosAsync();
        Task<Produto> ObterDetalheProdutoAsync(int id);
        Task AtualizarProdutoAsync(Produto produto);
        Task AdicionarProdutoAsync(Produto produto);
        Task ExcluirProdutoAsync(int produto);
        Task AtualizarMaisVendidosAsync(int topN);
        Task AtualizarImagemProdutoAsync(int produtoId, string caminhoImagem);

    }
}
