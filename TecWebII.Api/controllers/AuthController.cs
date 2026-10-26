using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecWebII.Api.Core;
using TecWebII.Api.Dtos.Auth;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Controllers;

/// <summary>Registro de usuarios, inicio de sesión y consulta del perfil autenticado.</summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[Tags("Autenticación")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registra un nuevo usuario y devuelve un token JWT.</summary>
    /// <response code="201">Usuario registrado correctamente.</response>
    /// <response code="409">El correo ya está registrado.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<AuthResponseDto>.Ok(response, "Usuario registrado correctamente."));
    }

    /// <summary>Inicia sesión con correo y contraseña.</summary>
    /// <response code="200">Credenciales válidas; devuelve el token JWT.</response>
    /// <response code="401">Credenciales inválidas o usuario inactivo.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(response, "Inicio de sesión exitoso."));
    }

    /// <summary>Devuelve el perfil del usuario autenticado a partir del token JWT.</summary>
    /// <response code="200">Perfil del usuario.</response>
    /// <response code="401">Token ausente, inválido o expirado.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(ApiResponse<object>.Fail("El token no contiene un identificador de usuario válido."));
        }

        var usuario = await _authService.GetProfileAsync(userId, cancellationToken);
        return Ok(ApiResponse<UsuarioDto>.Ok(usuario));
    }

    private bool TryGetUserId(out int userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out userId);
    }
}
