using TecWebII.Api.Dtos.Auth;

namespace TecWebII.Api.Services.Interfaces;

/// <summary>Contrato de autenticación de usuarios.</summary>
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<UsuarioDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
}
