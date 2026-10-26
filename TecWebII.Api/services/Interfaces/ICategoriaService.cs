using TecWebII.Api.Dtos.Categorias;

namespace TecWebII.Api.Services.Interfaces;

/// <summary>Contrato de la lógica de negocio de categorías.</summary>
public interface ICategoriaService
{
    Task<IReadOnlyList<CategoriaDto>> GetAllAsync(bool? activo = null, CancellationToken cancellationToken = default);
    Task<CategoriaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoriaDto> CreateAsync(CategoriaCreateDto request, CancellationToken cancellationToken = default);
    Task<CategoriaDto> UpdateAsync(int id, CategoriaUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
