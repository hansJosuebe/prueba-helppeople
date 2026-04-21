using CatalogoAPI.Models;

namespace CatalogoAPI.Services
{
    public interface IProductoService
    {
        Task<int> ProcesarExcelAsync(Stream archivoStream);
    }
}