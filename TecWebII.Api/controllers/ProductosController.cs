using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecWebII.Api.Core;
using TecWebII.Api.Dtos.Productos;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Controllers;

/// <summary>CRUD de productos con búsqueda, filtros, ordenamiento y paginación.</summary>
[ApiController]
[Route("api/productos")]
[Produces("application/json")]
[Tags("Productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>Lista los productos de forma paginada con filtros opcionales.</summary>
    /// <param name="query">Filtros, ordenamiento y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] ProductoQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _productoService.GetPagedAsync(query, cancellationToken);
        return Ok(ApiResponse<PagedResult<ProductoDto>>.Ok(result));
    }

    /// <summary>Obtiene un producto por su identificador.</summary>
    /// <response code="404">El producto no existe.</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var producto = await _productoService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<ProductoDto>.Ok(producto));
    }

    /// <summary>Crea un nuevo producto. Requiere autenticación.</summary>
    /// <response code="201">Producto creado.</response>
    /// <response code="409">El código de producto ya existe.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProductoCreateDto request, CancellationToken cancellationToken)
    {
        var producto = await _productoService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id },
            ApiResponse<ProductoDto>.Ok(producto, "Producto creado correctamente."));
    }

    /// <summary>Actualiza un producto existente. Requiere autenticación.</summary>
    /// <response code="200">Producto actualizado.</response>
    /// <response code="404">El producto no existe.</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ProductoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto request, CancellationToken cancellationToken)
    {
        var producto = await _productoService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<ProductoDto>.Ok(producto, "Producto actualizado correctamente."));
    }

    /// <summary>Elimina un producto. Requiere autenticación.</summary>
    /// <response code="204">Producto eliminado.</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _productoService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
