using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaquinaRefrescos.Infrastructure.Repositories;

public class VentaRepository : IVentaRepository
{
    private readonly AppDbContext _context;

    public VentaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Venta>> ObtenerHistorialAsync()
        => await _context.Ventas
            .Include(v => v.Producto)
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.Id)
            .ToListAsync();

    public async Task AgregarAsync(Venta venta)
        => await _context.Ventas.AddAsync(venta);
}
