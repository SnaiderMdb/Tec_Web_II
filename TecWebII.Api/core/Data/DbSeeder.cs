using Microsoft.EntityFrameworkCore;
using TecWebII.Api.Core;
using TecWebII.Api.Core.Security;
using TecWebII.Api.Models;

namespace TecWebII.Api.Core.Data;

/// <summary>Carga los datos iniciales de demostración (usuario, categorías y productos).</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        await context.Database.MigrateAsync();

        if (!await context.Usuarios.AnyAsync())
        {
            context.Usuarios.Add(new Usuario
            {
                NombreCompleto = "Snaider Administrador",
                Email = "admin@tecwebii.com",
                PasswordHash = passwordHasher.Hash("Admin123*"),
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
            logger.LogInformation("Usuario inicial creado: admin@tecwebii.com");
        }

        if (!await context.Categorias.AnyAsync())
        {
            var categorias = new List<Categoria>
            {
                new() { Nombre = "Laptops", Descripcion = "Equipos portátiles para trabajo y estudio." },
                new() { Nombre = "Accesorios", Descripcion = "Periféricos y accesorios tecnológicos." },
                new() { Nombre = "Componentes", Descripcion = "Partes internas para computadoras." }
            };

            context.Categorias.AddRange(categorias);
            await context.SaveChangesAsync();

            context.Productos.AddRange(
                new Producto
                {
                    Nombre = "Laptop Lenovo ThinkPad E14",
                    Descripcion = "Ryzen 5, 16 GB RAM, SSD 512 GB.",
                    Codigo = "LAP-001",
                    Precio = 899.99m,
                    Stock = 12,
                    CategoriaId = categorias[0].Id
                },
                new Producto
                {
                    Nombre = "Laptop HP Pavilion 14",
                    Descripcion = "Core i5, 8 GB RAM, SSD 256 GB.",
                    Codigo = "LAP-002",
                    Precio = 749.50m,
                    Stock = 8,
                    CategoriaId = categorias[0].Id
                },
                new Producto
                {
                    Nombre = "Mouse inalámbrico Logitech M185",
                    Descripcion = "Conexión USB, duración de batería 12 meses.",
                    Codigo = "ACC-001",
                    Precio = 19.99m,
                    Stock = 45,
                    CategoriaId = categorias[1].Id
                },
                new Producto
                {
                    Nombre = "Teclado mecánico Redragon Kumara",
                    Descripcion = "Switch red, retroiluminación LED.",
                    Codigo = "ACC-002",
                    Precio = 49.90m,
                    Stock = 20,
                    CategoriaId = categorias[1].Id
                },
                new Producto
                {
                    Nombre = "SSD Kingston NV2 1TB",
                    Descripcion = "Unidad NVMe PCIe 4.0.",
                    Codigo = "COM-001",
                    Precio = 84.75m,
                    Stock = 30,
                    CategoriaId = categorias[2].Id
                });

            await context.SaveChangesAsync();
            logger.LogInformation("Datos iniciales de categorías y productos cargados.");
        }
    }
}
