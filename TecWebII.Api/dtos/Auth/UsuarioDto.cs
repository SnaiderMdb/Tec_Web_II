namespace TecWebII.Api.Dtos.Auth;

/// <summary>Información pública del usuario autenticado.</summary>
public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
