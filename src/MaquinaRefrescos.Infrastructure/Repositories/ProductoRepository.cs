using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaquinaRefrescos.Infrastructure.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _context;

    public ProductoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Producto>> ObtenerTodosAsync()
        => await _context.Productos.OrderBy(p => p.Id).ToListAsync();

    public Task<Producto?> ObtenerPorIdAsync(int id)
        => _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
}
