using System.ComponentModel.DataAnnotations;

namespace CatalogoAPI.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Relación: Una categoría tiene muchos productos
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}