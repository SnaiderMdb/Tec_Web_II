using Microsoft.EntityFrameworkCore;
using TecWebII.Api.Core;
using TecWebII.Api.Core.Exceptions;
using TecWebII.Api.Dtos.Productos;
using TecWebII.Api.Models;
using TecWebII.Api.Services.Interfaces;

namespace TecWebII.Api.Services.Implementations;

/// <summary>Lógica de negocio de productos: filtros, ordenamiento, paginación y validaciones.</summary>
public class ProductoService : IProductoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(AppDbContext context, ILogger<ProductoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ProductoDto>> GetPagedAsync(ProductoQueryDto query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var productos = _context.Productos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            productos = productos.Where(p =>
                p.Nombre.ToLower().Contains(term) ||
                p.Codigo.ToLower().Contains(term) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(term)));
        }

        if (query.CategoriaId.HasValue)
        {
            productos = productos.Where(p => p.CategoriaId == query.CategoriaId.Value);
        }

        if (query.Activo.HasValue)
        {
            productos = productos.Where(p => p.Activo == query.Activo.Value);
        }

        if (query.PrecioMin.HasValue)
        {
            productos = productos.Where(p => p.Precio >= query.PrecioMin.Value);
        }

        if (query.PrecioMax.HasValue)
        {
            productos = productos.Where(p => p.Precio <= query.PrecioMax.Value);
        }

        var totalItems = await productos.CountAsync(cancellationToken);

        var descending = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        productos = query.SortBy.Trim().ToLowerInvariant() switch
        {
            "precio" => descending ? productos.OrderByDescending(p => p.Precio) : productos.OrderBy(p => p.Precio),
            "stock" => descending ? productos.OrderByDescending(p => p.Stock) : productos.OrderBy(p => p.Stock),
            "codigo" => descending ? productos.OrderByDescending(p => p.Codigo) : productos.OrderBy(p => p.Codigo),
            "fecha" => descending ? productos.OrderByDescending(p => p.FechaCreacion) : productos.OrderBy(p => p.FechaCreacion),
            _ => descending ? productos.OrderByDescending(p => p.Nombre) : productos.OrderBy(p => p.Nombre)
        };

        var items = await productos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Codigo = p.Codigo,
                Precio = p.Precio,
                Stock = p.Stock,
                Activo = p.Activo,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : string.Empty,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductoDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }

    public async Task<ProductoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var producto = await _context.Productos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Codigo = p.Codigo,
                Precio = p.Precio,
                Stock = p.Stock,
                Activo = p.Activo,
                CategoriaId = p.CategoriaId,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : string.Empty,
                FechaCreacion = p.FechaCreacion,
                FechaActualizacion = p.FechaActualizacion
            })
            .FirstOrDefaultAsync(cancellationToken);

        return producto ?? throw new NotFoundException($"No se encontró el producto con id {id}.");
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto request, CancellationToken cancellationToken = default)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        await EnsureCodigoDisponibleAsync(codigo, null, cancellationToken);
        var categoria = await GetCategoriaAsync(request.CategoriaId, cancellationToken);

        var producto = new Producto
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = Normalize(request.Descripcion),
            Codigo = codigo,
            Precio = request.Precio,
            Stock = request.Stock,
            Activo = request.Activo,
            CategoriaId = categoria.Id,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Producto creado: {Id} - {Codigo}", producto.Id, producto.Codigo);

        return MapToDto(producto, categoria.Nombre);
    }

    public async Task<ProductoDto> UpdateAsync(int id, ProductoUpdateDto request, CancellationToken cancellationToken = default)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el producto con id {id}.");

        var codigo = request.Codigo.Trim().ToUpperInvariant();
        await EnsureCodigoDisponibleAsync(codigo, id, cancellationToken);
        var categoria = await GetCategoriaAsync(request.CategoriaId, cancellationToken);

        producto.Nombre = request.Nombre.Trim();
        producto.Descripcion = Normalize(request.Descripcion);
        producto.Codigo = codigo;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        producto.Activo = request.Activo;
        producto.CategoriaId = categoria.Id;
        producto.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Producto actualizado: {Id} - {Codigo}", producto.Id, producto.Codigo);

        return MapToDto(producto, categoria.Nombre);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el producto con id {id}.");

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Producto eliminado: {Id} - {Codigo}", producto.Id, producto.Codigo);
    }

    private async Task<Categoria> GetCategoriaAsync(int categoriaId, CancellationToken cancellationToken)
    {
        return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoriaId, cancellationToken)
            ?? throw new AppException($"La categoría con id {categoriaId} no existe.", StatusCodes.Status400BadRequest);
    }

    private async Task EnsureCodigoDisponibleAsync(string codigo, int? excludeId, CancellationToken cancellationToken)
    {
        var exists = await _context.Productos.AnyAsync(
            p => p.Codigo == codigo && (!excludeId.HasValue || p.Id != excludeId.Value),
            cancellationToken);

        if (exists)
        {
            throw new ConflictException($"Ya existe un producto con el código '{codigo}'.");
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ProductoDto MapToDto(Producto producto, string categoriaNombre) => new()
    {
        Id = producto.Id,
        Nombre = producto.Nombre,
        Descripcion = producto.Descripcion,
        Codigo = producto.Codigo,
        Precio = producto.Precio,
        Stock = producto.Stock,
        Activo = producto.Activo,
        CategoriaId = producto.CategoriaId,
        CategoriaNombre = categoriaNombre,
        FechaCreacion = producto.FechaCreacion,
        FechaActualizacion = producto.FechaActualizacion
    };
}
