using Microsoft.AspNetCore.Mvc;
using WSM.ApiECommerce.Repositories;

namespace WSM.ApiECommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaRepository categoriaRepository;

    public CategoriasController(ICategoriaRepository categoriaRepository)
    {
        this.categoriaRepository = categoriaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categorias = await categoriaRepository.GetCategorias();
        return Ok(categorias);
    }
}
