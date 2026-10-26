using System.ComponentModel.DataAnnotations;

namespace TecWebII.Api.Dtos.Productos;

/// <summary>Datos para actualizar un producto existente.</summary>
public class ProductoUpdateDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El código debe tener entre 2 y 30 caracteres.")]
    [RegularExpression(@"^[A-Za-z0-9\-_]+$", ErrorMessage = "El código solo admite letras, números, guiones y guiones bajos.")]
    public string Codigo { get; set; } = string.Empty;

    [Range(0.01, 9_999_999.99, ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal Precio { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
    public int CategoriaId { get; set; }

    public bool Activo { get; set; } = true;
}
