using System.ComponentModel.DataAnnotations;

namespace TecWebII.Api.Dtos.Productos;

/// <summary>Parámetros de búsqueda, ordenamiento y paginación para el listado de productos.</summary>
public class ProductoQueryDto
{
    /// <summary>Texto libre que se busca en nombre, descripción o código.</summary>
    [StringLength(100)]
    public string? Search { get; set; }

    public int? CategoriaId { get; set; }

    public bool? Activo { get; set; }

    [Range(0, 9_999_999.99, ErrorMessage = "El precio mínimo no puede ser negativo.")]
    public decimal? PrecioMin { get; set; }

    [Range(0, 9_999_999.99, ErrorMessage = "El precio máximo no puede ser negativo.")]
    public decimal? PrecioMax { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor o igual a 1.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int PageSize { get; set; } = 10;

    /// <summary>Campo de ordenamiento: nombre, precio, stock, codigo o fecha.</summary>
    public string SortBy { get; set; } = "nombre";

    /// <summary>Dirección del ordenamiento: asc o desc.</summary>
    public string SortDir { get; set; } = "asc";
}
