using System.ComponentModel.DataAnnotations;

namespace TecWebII.Api.Core.Security;

/// <summary>Configuración del emisor de tokens JWT.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "El emisor del token es obligatorio.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "La audiencia del token es obligatoria.")]
    public string Audience { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clave secreta es obligatoria.")]
    [MinLength(32, ErrorMessage = "La clave secreta debe tener al menos 32 caracteres.")]
    public string Key { get; set; } = string.Empty;

    [Range(5, 1440, ErrorMessage = "La expiración debe estar entre 5 y 1440 minutos.")]
    public int ExpirationMinutes { get; set; } = 120;
}
