namespace TecWebII.Api.Dtos.Categorias;

/// <summary>Categoría tal como se devuelve al cliente.</summary>
public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CantidadProductos { get; set; }
}
