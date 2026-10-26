using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecWebII.Api.Core;
using TecWebII.Api.Dtos.Categorias;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Controllers;

/// <summary>CRUD de categorías del catálogo.</summary>
[ApiController]
[Route("api/categorias")]
[Produces("application/json")]
[Tags("Categorías")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>Lista todas las categorías; permite filtrar por estado.</summary>
    /// <param name="activo">true para activas, false para inactivas, omitir para todas.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CategoriaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? activo, CancellationToken cancellationToken)
    {
        var categorias = await _categoriaService.GetAllAsync(activo, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<CategoriaDto>>.Ok(categorias));
    }

    /// <summary>Obtiene una categoría por su identificador.</summary>
    /// <response code="404">La categoría no existe.</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CategoriaDto>.Ok(categoria));
    }

    /// <summary>Crea una nueva categoría. Requiere autenticación.</summary>
    /// <response code="201">Categoría creada.</response>
    /// <response code="409">Ya existe una categoría con ese nombre.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CategoriaCreateDto request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id },
            ApiResponse<CategoriaDto>.Ok(categoria, "Categoría creada correctamente."));
    }

    /// <summary>Actualiza una categoría existente. Requiere autenticación.</summary>
    /// <response code="200">Categoría actualizada.</response>
    /// <response code="404">La categoría no existe.</response>
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateDto request, CancellationToken cancellationToken)
    {
        var categoria = await _categoriaService.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResponse<CategoriaDto>.Ok(categoria, "Categoría actualizada correctamente."));
    }

    /// <summary>Elimina una categoría sin productos asociados. Requiere autenticación.</summary>
    /// <response code="204">Categoría eliminada.</response>
    /// <response code="409">La categoría tiene productos asociados.</response>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _categoriaService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
