using Microsoft.AspNetCore.Mvc;
using CatalogoAPI.Models;
using CatalogoAPI.Repositories;

namespace CatalogoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;

        public CategoriasController(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias() => 
            Ok(await _repository.GetAllAsync());

        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            if (await _repository.ExisteNombreAsync(categoria.Nombre))
                return BadRequest(new { message = "El nombre de la categoría ya existe." });

            await _repository.CreateAsync(categoria);
            return CreatedAtAction(nameof(GetCategorias), new { id = categoria.IdCategoria }, categoria);
        }
    }
}