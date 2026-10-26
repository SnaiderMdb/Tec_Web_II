using Microsoft.EntityFrameworkCore;
using TecWebII.Api.Core;
using TecWebII.Api.Core.Exceptions;
using TecWebII.Api.Core.Security;
using TecWebII.Api.Dtos.Auth;
using TecWebII.Api.Models;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Services.Implementations;

/// <summary>Implementa el registro, inicio de sesión y perfil de usuarios.</summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _context.Usuarios.AnyAsync(u => u.Email == email, cancellationToken);
        if (exists)
        {
            throw new ConflictException($"Ya existe un usuario registrado con el correo '{email}'.");
        }

        var usuario = new Usuario
        {
            NombreCompleto = request.NombreCompleto.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Rol = "Usuario",
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario {Email} registrado con id {UserId}.", usuario.Email, usuario.Id);

        return BuildAuthResponse(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (usuario is null || !_passwordHasher.Verify(request.Password, usuario.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales inválidas. Verifique el correo y la contraseña.");
        }

        if (!usuario.Activo)
        {
            throw new UnauthorizedException("El usuario se encuentra inactivo. Contacte al administrador.");
        }

        _logger.LogInformation("Inicio de sesión exitoso para {Email}.", usuario.Email);

        return BuildAuthResponse(usuario);
    }

    public async Task<UsuarioDto> GetProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("El usuario autenticado no existe.");

        return MapToDto(usuario);
    }

    private AuthResponseDto BuildAuthResponse(Usuario usuario)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(usuario);

        return new AuthResponseDto
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresAt = expiresAt,
            Usuario = MapToDto(usuario)
        };
    }

    private static UsuarioDto MapToDto(Usuario usuario) => new()
    {
        Id = usuario.Id,
        NombreCompleto = usuario.NombreCompleto,
        Email = usuario.Email,
        Rol = usuario.Rol,
        FechaCreacion = usuario.FechaCreacion
    };
}
