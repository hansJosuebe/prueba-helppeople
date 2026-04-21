using CatalogoAPI.Models;

namespace CatalogoAPI.Repositories
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task CreateAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre); // Para validar unicidad
    }
}