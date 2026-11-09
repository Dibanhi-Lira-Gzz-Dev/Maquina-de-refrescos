using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Infrastructure.Data;

namespace MaquinaRefrescos.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IProductoRepository productos,
        IMonederoRepository monedero,
        IVentaRepository ventas)
    {
        _context = context;
        Productos = productos;
        Monedero = monedero;
        Ventas = ventas;
    }

    public IProductoRepository Productos { get; }
    public IMonederoRepository Monedero { get; }
    public IVentaRepository Ventas { get; }

    public Task<int> GuardarAsync() => _context.SaveChangesAsync();
}
