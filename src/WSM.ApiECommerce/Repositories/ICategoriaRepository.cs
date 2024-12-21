using WSM.ApiECommerce.Entities;

namespace WSM.ApiECommerce.Repositories;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetCategorias();
}
