using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TecWebII.Api.Core.Security;
using TecWebII.Api.Models;

namespace TecWebII.Api.Services.Interfaces;

/// <summary>Contrato del generador de tokens JWT.</summary>
public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(Usuario usuario);
}
