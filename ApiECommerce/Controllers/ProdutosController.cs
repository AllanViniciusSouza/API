using ApiECommerce.Entities;
using ApiECommerce.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;


namespace ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutosController(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProdutos(string tipoProduto, int? categoriaId = null)
    {
        IEnumerable<Produto> produtos;

        if (tipoProduto == "categoria" && categoriaId != null)
        {
            produtos = await _produtoRepository.ObterProdutosPorCategoriaAsync(categoriaId.Value);
        }
        else if (tipoProduto == "popular")
        {
            produtos = await _produtoRepository.ObterProdutosPopularesAsync();
        }
        else if (tipoProduto == "maisvendido")
        {
            produtos = await _produtoRepository.ObterProdutosMaisVendidosAsync();
        }
        else
        {
            return BadRequest("Tipo de produto inválido");
        }

        var dadosProduto = produtos.Select(v => new
        {
            Id = v.Id,
            Nome = v.Nome,
            Preco = v.Preco,
            UrlImagem = v.UrlImagem,
            CategoriaId = v.CategoriaId
        });

        return Ok(dadosProduto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetalheProduto(int id)
    {
        var produto = await _produtoRepository.ObterDetalheProdutoAsync(id);

        if (produto is null)
        {
            return NotFound($"Produto com id={id} não encontrado");
        }

        var dadosProduto = new
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Preco = produto.Preco,
            Detalhe = produto.Detalhe,
            UrlImagem = produto.UrlImagem
        };

        return Ok(dadosProduto);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetTodosProdutos()
    {
        IEnumerable<Produto> produtos;

        produtos = await _produtoRepository.ObterTodosProdutosAsync();

        var dadosProduto = produtos.Select(v => new
        {
            Id = v.Id,
            Nome = v.Nome,
            Preco = v.Preco,
            Barcode = v.Barcode,
            UrlImagem = v.UrlImagem,
            CategoriaId = v.CategoriaId,
            Popular = v.Popular,
            DiasDisponiveis = v.DiasDisponiveis,
            Disponivel = v.Disponivel
        });

        return Ok(dadosProduto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
    {
        if (produto == null || id != produto.Id)
        {
            return BadRequest("Os dados do produto são inválidos ou o ID não corresponde.");
        }

        try
        {
            produto.AtualizarDisponibilidade(); // 👈 calcula antes de salvar
            await _produtoRepository.AtualizarProdutoAsync(produto);
            return Ok(new { Message = "Produto atualizado com sucesso!", produto });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }


    [HttpPost]
    public async Task<IActionResult> AdicionarProduto([FromBody] Produto novoProduto)
    {
        if (novoProduto == null)
        {
            return BadRequest("Os dados do produto são inválidos.");
        }

        try
        {
            novoProduto.AtualizarDisponibilidade(); // 👈 calcula antes de salvar
            await _produtoRepository.AdicionarProdutoAsync(novoProduto);
            return CreatedAtAction(nameof(GetDetalheProduto), new { id = novoProduto.Id }, novoProduto);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletarProduto(int id)
    {
        try
        {
            var produto = await _produtoRepository.ObterDetalheProdutoAsync(id);
            if (produto == null)
            {
                return NotFound($"Produto com id={id} não encontrado.");
            }

            await _produtoRepository.ExcluirProdutoAsync(id);
            return NoContent(); // Retorna 204 No Content
        }
        catch (Exception ex)
        {
            // Aqui você pode logar o erro conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpGet("[action]/{topN}")]
    public async Task<IActionResult> AtualizarMaisVendidos(int topN)
    {
        if (topN == null)
        {
            return BadRequest("Insira quantos produtos.");
        }

        try
        {
            await _produtoRepository.AtualizarMaisVendidosAsync(topN);
            return Ok(new { Message = "Mais vendidos atualizado com sucesso!"});
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> MaisVendidos()
    {
        try
        {
            IEnumerable<Produto> produtos;

            produtos = await _produtoRepository.ObterProdutosMaisVendidosAsync();

            var dadosProduto = produtos.Select(v => new
            {
                Id = v.Id,
                Nome = v.Nome,
                Preco = v.Preco,
                Barcode = v.Barcode,
                UrlImagem = v.UrlImagem,
                CategoriaId = v.CategoriaId,
                Popular = v.Popular,
                DiasDisponiveis = v.DiasDisponiveis,
                Disponivel = v.Disponivel
            });

            return Ok(dadosProduto);
        }
        catch (Exception ex)
        {
            // Logar o erro (ex) conforme necessário
            return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
        }
    }

    [HttpPut("{id}/foto")]
    public async Task<IActionResult> AtualizarFotoProduto(int id, IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Arquivo inválido.");

        using var image = await SixLabors.ImageSharp.Image.LoadAsync(arquivo.OpenReadStream()); // SixLabors.ImageSharp
        image.Mutate(x => x.Resize(225, 225));

        var nomeArquivo = $"produto_{id}.jpg";
        var caminho = Path.Combine("wwwroot", nomeArquivo);

        await image.SaveAsJpegAsync(caminho);

        // Atualize no banco (ex: produto.CaminhoImagem = $"/imagens/produtos/{nomeArquivo}")
        await _produtoRepository.AtualizarImagemProdutoAsync(id, $"/{nomeArquivo}");

        return Ok();
    }

        

}
