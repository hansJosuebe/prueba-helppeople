using ClosedXML.Excel;
using CatalogoAPI.Models;
using CatalogoAPI.Repositories;

namespace CatalogoAPI.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> ProcesarExcelAsync(Stream archivoStream)
        {
            using var workbook = new XLWorkbook(archivoStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);
            int procesados = 0;

            foreach (var row in rows)
            {
                var nuevoProducto = new Producto
                {
                    Nombre = row.Cell(1).GetValue<string>(),
                    Precio = row.Cell(2).GetValue<decimal>(),
                    Stock = row.Cell(3).GetValue<int>(),
                    IdCategoria = row.Cell(4).GetValue<int>(),
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now
                };

                await _repository.CreateAsync(nuevoProducto);
                procesados++;
            }
            return procesados;
        }
    }
}