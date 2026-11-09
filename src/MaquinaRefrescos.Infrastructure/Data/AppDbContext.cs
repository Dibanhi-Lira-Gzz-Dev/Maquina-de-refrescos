using MaquinaRefrescos.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaquinaRefrescos.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Moneda> Monedero => Set<Moneda>();
    public DbSet<Venta> Ventas => Set<Venta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Precio).HasPrecision(10, 2);
            entity.Property(p => p.Existencia).IsRequired();
        });

        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.ToTable("Monedero");
            entity.HasKey(m => m.Denominacion);
            entity.Property(m => m.Denominacion).HasPrecision(10, 2).ValueGeneratedNever();
            entity.Property(m => m.Cantidad).IsRequired();
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("Ventas");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Precio).HasPrecision(10, 2);
            entity.Property(v => v.MontoIngresado).HasPrecision(10, 2);
            entity.Property(v => v.Cambio).HasPrecision(10, 2);
            entity.Property(v => v.MonedasIngresadas).IsRequired().HasMaxLength(200);
            entity.Property(v => v.MonedasCambio).IsRequired().HasMaxLength(200);
            entity.Property(v => v.Fecha).IsRequired();

            entity.HasOne(v => v.Producto)
                .WithMany()
                .HasForeignKey(v => v.ProductoId);
        });
    }
}
