using CatalogoAPI.Data;
using CatalogoAPI.Models;
using CatalogoAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponseDTO<Producto>> GetProductosAsync(int page, int pageSize, string? search, int? idCategoria, decimal? precioMin, decimal? precioMax)
        {
            // Requisito: Solo traer los que no están eliminados (Soft Delete)
            var query = _context.Productos
                .Where(p => p.Activo == true)
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Nombre.Contains(search));

            if (idCategoria.HasValue)
                query = query.Where(p => p.IdCategoria == idCategoria.Value);

            if (precioMin.HasValue)
                query = query.Where(p => p.Precio >= precioMin.Value);

            if (precioMax.HasValue)
                query = query.Where(p => p.Precio <= precioMax.Value);

            var total = await query.CountAsync();
            
            var items = await query
                .OrderBy(p => p.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponseDTO<Producto>
            {
                Items = items,
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<Producto?> GetByIdAsync(int id) => await _context.Productos.FindAsync(id);
        
        public async Task CreateAsync(Producto producto) 
        { 
            _context.Productos.Add(producto); 
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(Producto producto) 
        { 
            // Forma segura de actualizar para evitar errores de rastreo (tracking)
            var existente = await _context.Productos.FindAsync(producto.IdProducto);
            if (existente != null)
            {
                existente.Nombre = producto.Nombre;
                existente.Precio = producto.Precio;
                existente.Stock = producto.Stock;
                existente.IdCategoria = producto.IdCategoria;
                existente.FechaModificacion = DateTime.Now; // Auditoría obligatoria

                await _context.SaveChangesAsync(); 
            }
        }

       public async Task DeleteAsync(int id) 
{ 
    var p = await _context.Productos.FindAsync(id); 
    
    if (p != null) 
    { 
        if (p.Stock > 1) 
        {
            // Si hay más de uno, solo descontamos 1
            p.Stock -= 1;
        }
        else 
        {
            // Si queda el último o ya está en cero, lo inactivamos
            p.Stock = 0;
            p.Activo = false;
        }

        p.FechaModificacion = DateTime.Now; // Auditoría
        await _context.SaveChangesAsync(); 
    }
}
    }
}