namespace TecWebII.Api.Dtos.Auth;

/// <summary>Respuesta entregada tras un login o registro exitoso.</summary>
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
}
