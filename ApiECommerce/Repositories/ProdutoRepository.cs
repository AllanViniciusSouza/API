using ApiECommerce.Context;
using ApiECommerce.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _dbContext;

    public ProdutoRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Produto>> ObterProdutosPorCategoriaAsync(int categoriaId)
    {
        return await _dbContext.Produtos
            .Where(p => p.CategoriaId == categoriaId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> ObterProdutosPopularesAsync()
    {
        return await _dbContext.Produtos
            .Where(p => p.Popular)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> ObterProdutosMaisVendidosAsync()
    {
        return await _dbContext.Produtos
            .Where(p => p.MaisVendido)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> ObterTodosProdutosAsync()
    {
        return await _dbContext.Produtos
            .ToListAsync();
    }

    public async Task<Produto> ObterDetalheProdutoAsync(int id)
    {
        var detalheProduto =  await _dbContext.Produtos
                                              .FirstOrDefaultAsync(p => p.Id == id);

        if (detalheProduto is null)
            throw new InvalidOperationException();

        return detalheProduto;
    }

    public async Task AtualizarProdutoAsync(Produto produto)
    {
        var produtoExistente = await _dbContext.Produtos.FindAsync(produto.Id);

        if (produtoExistente == null)
        {
            throw new KeyNotFoundException("Produto não encontrado");
        }

        // Atualiza apenas os campos necessários
        produtoExistente.Nome = produto.Nome;
        produtoExistente.Preco = produto.Preco;
        produtoExistente.PrecoCusto = produto.PrecoCusto;
        produtoExistente.PrecoQuente = produto.PrecoQuente;
        produtoExistente.PrecoGelada = produto.PrecoGelada;
        produtoExistente.PrecoEntrega = produto.PrecoEntrega;
        produtoExistente.PrecoRetirar = produto.PrecoRetirar;
        produtoExistente.Barcode = produto.Barcode;
        produtoExistente.Popular = produto.Popular;
        produtoExistente.CategoriaId = produto.CategoriaId;
        produtoExistente.Disponivel = produto.Disponivel;
        produtoExistente.DiasDisponiveis = produto.DiasDisponiveis;

        await _dbContext.SaveChangesAsync();
    }


    public async Task AdicionarProdutoAsync(Produto produto)
    {
        produto.Detalhe = produto.Nome;
        _dbContext.Produtos.Add(produto); // Adiciona o novo produto ao DbSet
        await _dbContext.SaveChangesAsync(); // Salva as alterações no banco de dados
    }

    public async Task ExcluirProdutoAsync(int id)
    {
        var produto = await _dbContext.Produtos.FindAsync(id);
        if (produto != null)
        {
            produto.Disponivel = false;
            //_dbContext.Produtos.Remove(produto);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            // Produto não encontrado; você pode lançar uma exceção ou simplesmente retornar.
        }
    }

    public async Task AtualizarMaisVendidosAsync(int topN = 5)
    {
        // 1. Agrupar movimentações de SAÍDA por ProdutoId e somar a quantidade
        var movimentacoesSaida = await _dbContext.MovimentacoesEstoque
            .Where(m => m.Tipo == TipoMovimentacao.Venda)
            .GroupBy(m => m.ProdutoId)
            .Select(g => new
            {
                ProdutoId = g.Key,
                QuantidadeSaida = g.Sum(m => m.Quantidade)
            })
            .OrderByDescending(g => g.QuantidadeSaida)
            .Take(topN)
            .ToListAsync();

        // 2. Obter os IDs dos mais vendidos
        var idsMaisVendidos = movimentacoesSaida.Select(m => m.ProdutoId).ToList();

        // 3. Buscar todos os produtos
        var todosProdutos = await _dbContext.Produtos.ToListAsync();

        foreach (var produto in todosProdutos)
        {
            produto.MaisVendido = idsMaisVendidos.Contains(produto.Id);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task AtualizarImagemProdutoAsync(int produtoId, string caminhoImagem)
    {
        var produto = await _dbContext.Produtos.FindAsync(produtoId);
        if (produto == null)
            throw new Exception("Produto não encontrado");

        produto.UrlImagem = caminhoImagem;
        await _dbContext.SaveChangesAsync();
    }

}
