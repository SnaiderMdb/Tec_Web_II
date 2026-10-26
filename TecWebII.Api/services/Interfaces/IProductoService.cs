using TecWebII.Api.Core;
using TecWebII.Api.Dtos.Productos;

namespace TecWebII.Api.Services.Interfaces;

/// <summary>Contrato de la lógica de negocio de productos.</summary>
public interface IProductoService
{
    Task<PagedResult<ProductoDto>> GetPagedAsync(ProductoQueryDto query, CancellationToken cancellationToken = default);
    Task<ProductoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductoDto> CreateAsync(ProductoCreateDto request, CancellationToken cancellationToken = default);
    Task<ProductoDto> UpdateAsync(int id, ProductoUpdateDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
