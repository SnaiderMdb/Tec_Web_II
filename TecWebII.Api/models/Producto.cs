using System.ComponentModel.DataAnnotations;

namespace TecWebII.Api.Models;

/// <summary>
/// Representa un producto del catálogo. Cada producto pertenece a una categoría.
/// </summary>
public class Producto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 30 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Range(0.01, 9_999_999.99, ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public bool Activo { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaActualizacion { get; set; }
}
