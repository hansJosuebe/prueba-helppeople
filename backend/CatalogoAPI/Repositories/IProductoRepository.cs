using CatalogoAPI.Models;
using CatalogoAPI.DTOs;

namespace CatalogoAPI.Repositories
{
    public interface IProductoRepository
    {
        Task<PagedResponseDTO<Producto>> GetProductosAsync(int page, int pageSize, string? search, int? idCategoria, decimal? precioMin, decimal? precioMax);
        Task<Producto?> GetByIdAsync(int id);
        Task CreateAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
    }
}