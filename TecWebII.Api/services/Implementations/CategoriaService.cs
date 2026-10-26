using Microsoft.EntityFrameworkCore;
using TecWebII.Api.Core;
using TecWebII.Api.Core.Exceptions;
using TecWebII.Api.Dtos.Categorias;
using TecWebII.Api.Models;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Services.Implementations;

/// <summary>Lógica de negocio de categorías con validaciones y mapeo a DTOs.</summary>
public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(AppDbContext context, ILogger<CategoriaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IReadOnlyList<CategoriaDto>> GetAllAsync(bool? activo = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categorias.AsNoTracking();

        if (activo.HasValue)
        {
            query = query.Where(c => c.Activo == activo.Value);
        }

        return await query
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Activo = c.Activo,
                FechaCreacion = c.FechaCreacion,
                CantidadProductos = c.Productos.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoriaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                Activo = c.Activo,
                FechaCreacion = c.FechaCreacion,
                CantidadProductos = c.Productos.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        return categoria ?? throw new NotFoundException($"No se encontró la categoría con id {id}.");
    }

    public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto request, CancellationToken cancellationToken = default)
    {
        var nombre = request.Nombre.Trim();
        await EnsureNombreDisponibleAsync(nombre, null, cancellationToken);

        var categoria = new Categoria
        {
            Nombre = nombre,
            Descripcion = Normalize(request.Descripcion),
            Activo = request.Activo,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Categoría creada: {Id} - {Nombre}", categoria.Id, categoria.Nombre);

        return MapToDto(categoria);
    }

    public async Task<CategoriaDto> UpdateAsync(int id, CategoriaUpdateDto request, CancellationToken cancellationToken = default)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la categoría con id {id}.");

        var nombre = request.Nombre.Trim();
        await EnsureNombreDisponibleAsync(nombre, id, cancellationToken);

        categoria.Nombre = nombre;
        categoria.Descripcion = Normalize(request.Descripcion);
        categoria.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Categoría actualizada: {Id} - {Nombre}", categoria.Id, categoria.Nombre);

        var cantidadProductos = await _context.Productos.CountAsync(p => p.CategoriaId == id, cancellationToken);
        return MapToDto(categoria, cantidadProductos);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la categoría con id {id}.");

        var tieneProductos = await _context.Productos.AnyAsync(p => p.CategoriaId == id, cancellationToken);
        if (tieneProductos)
        {
            throw new ConflictException("No se puede eliminar la categoría porque tiene productos asociados.");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Categoría eliminada: {Id} - {Nombre}", categoria.Id, categoria.Nombre);
    }

    private async Task EnsureNombreDisponibleAsync(string nombre, int? excludeId, CancellationToken cancellationToken)
    {
        var normalized = nombre.ToLower();
        var exists = await _context.Categorias.AnyAsync(
            c => c.Nombre.ToLower() == normalized && (!excludeId.HasValue || c.Id != excludeId.Value),
            cancellationToken);

        if (exists)
        {
            throw new ConflictException($"Ya existe una categoría con el nombre '{nombre}'.");
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static CategoriaDto MapToDto(Categoria categoria, int cantidadProductos = 0) => new()
    {
        Id = categoria.Id,
        Nombre = categoria.Nombre,
        Descripcion = categoria.Descripcion,
        Activo = categoria.Activo,
        FechaCreacion = categoria.FechaCreacion,
        CantidadProductos = cantidadProductos
    };
}
