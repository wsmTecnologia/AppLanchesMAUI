using Microsoft.EntityFrameworkCore;
using WSM.ApiECommerce.Context;
using WSM.ApiECommerce.Entities;

namespace WSM.ApiECommerce.Repositories;

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

    public async Task<Produto> ObterDetalheProdutoAsync(int id)
    {
        var detalheProduto =  await _dbContext.Produtos
                                              .FirstOrDefaultAsync(p => p.Id == id);

        if (detalheProduto is null)
            throw new InvalidOperationException();

        return detalheProduto;
    }
}
