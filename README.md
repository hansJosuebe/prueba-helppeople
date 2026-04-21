Instrucciones de Ejecución
Descripción
Aplicación web de catálogo para administración de categorías y productos con .NET 8, React (Ant Design) y SQL Server.
Requisitos Previos
.NET 8 SDK
Node.js (v18+)
SQL Server
Pasos para Ejecución
Base de Datos: Ejecutar el script SQL adjunto en este documento en SQL Server Management Studio.
Backend: - Ir a /backend/CatalogoAPI.
Configurar appsettings.json con su cadena de conexión.
Ejecutar dotnet run.
Frontend: - Ir a /frontend.
Ejecutar npm install.
Ejecutar npm run dev.
2. SCRIPT SQL (Base de Datos)
-- Creación de la Base de Datos
CREATE DATABASE CatalogoDB;
GO
USE CatalogoDB;
GO

-- Tabla de Categorías
CREATE TABLE Categorias (
    IdCategoria INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(255),
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME NULL
);

-- Tabla de Productos
CREATE TABLE Productos (
    IdProducto INT PRIMARY KEY IDENTITY(1,1),
    IdCategoria INT NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL CHECK (Precio > 0),
    Stock INT DEFAULT 0,
    Activo BIT DEFAULT 1,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME NULL,
    CONSTRAINT FK_Productos_Categorias FOREIGN KEY (IdCategoria) REFERENCES Categorias(IdCategoria)
);

-- Índices para optimización de filtros (Requisito PDF)
CREATE INDEX IX_Productos_Nombre ON Productos(Nombre);
CREATE INDEX IX_Productos_Precio ON Productos(Precio);


3. CÓDIGO CLAVE DEL BACKEND (C#)
ProductoRepository.cs (Lógica de Negocio y Auditoría)
// Implementación de Soft Delete y actualización con auditoría
public async Task UpdateAsync(Producto producto) 
{ 
    var existente = await _context.Productos.FindAsync(producto.IdProducto);
    if (existente != null)
    {
        existente.Nombre = producto.Nombre;
        existente.Precio = producto.Precio;
        existente.Stock = producto.Stock;
        existente.IdCategoria = producto.IdCategoria;
        existente.FechaModificacion = DateTime.Now; // Auditoría automática
        await _context.SaveChangesAsync(); 
    }
}

public async Task DeleteAsync(int id) 
{ 
    var p = await _context.Productos.FindAsync(id); 
    if (p != null) 
    { 
        // Lógica de salida de inventario: resta 1 o inactiva si es el último
        if (p.Stock > 1) p.Stock -= 1;
        else { p.Stock = 0; p.Activo = false; }
        p.FechaModificacion = DateTime.Now;
        await _context.SaveChangesAsync(); 
    }
}


4. JUSTIFICACIÓN TÉCNICA
Soft Delete: Se implementó Activo = false para mantener la integridad referencial en reportes históricos.
Paginación: Realizada en SQL Server mediante Skip y Take para evitar sobrecarga de memoria en el servidor.
Índices: Se crearon índices en Nombre y Precio para acelerar las búsquedas por contiene y rangos solicitadas en el PDF.
