using Microsoft.AspNetCore.Mvc;
using CatalogoAPI.Models;
using CatalogoAPI.Repositories;
using CatalogoAPI.DTOs;
using CatalogoAPI.Services;

namespace CatalogoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _repository;
        private readonly ProductoService _productoService;

        public ProductosController(IProductoRepository repository, ProductoService productoService)
        {
            _repository = repository;
            _productoService = productoService;
        }

        // Listado con paginación y filtros desde el backend (Requisito obligatorio)
        [HttpGet]
        public async Task<ActionResult<PagedResponseDTO<Producto>>> GetProductos(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? search = null, 
            [FromQuery] int? idCategoria = null, 
            [FromQuery] decimal? precioMin = null, 
            [FromQuery] decimal? precioMax = null)
        {
            var result = await _repository.GetProductosAsync(page, pageSize, search, idCategoria, precioMin, precioMax);
            return Ok(result);
        }

        // Crear producto con validación y auditoría
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (producto.Precio <= 0) return BadRequest("El precio debe ser mayor a 0");
            
            producto.FechaCreacion = DateTime.Now; // Auditoría automática
            await _repository.CreateAsync(producto);
            return Ok(producto);
        }

        // Actualizar producto con auditoría
        [HttpPut("{id}")]
public async Task<IActionResult> PutProducto(int id, Producto producto)
{
    // Forzamos que el ID de la URL sea el mismo del objeto
    producto.IdProducto = id; 

    await _repository.UpdateAsync(producto);
    return NoContent();
}

        // Eliminar producto
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // Endpoint para carga masiva (Requisito obligatorio)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Selecciona un archivo válido.");

            try 
            {
                using var stream = file.OpenReadStream();
                int procesados = await _productoService.ProcesarExcelAsync(stream);
                return Ok(new { mensaje = $"Se procesaron {procesados} productos." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}