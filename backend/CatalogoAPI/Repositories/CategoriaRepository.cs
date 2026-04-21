using CatalogoAPI.Data;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetAllAsync() => 
            await _context.Categorias.Where(c => c.Activo).ToListAsync();

        public async Task<Categoria?> GetByIdAsync(int id) => 
            await _context.Categorias.FindAsync(id);

        public async Task CreateAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            _context.Entry(categoria).State = EntityState.Modified;
            categoria.FechaModificacion = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                // Soft delete: en lugar de borrar, desactivamos
                categoria.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre) => 
            await _context.Categorias.AnyAsync(c => c.Nombre.ToLower() == nombre.ToLower());
    }
}