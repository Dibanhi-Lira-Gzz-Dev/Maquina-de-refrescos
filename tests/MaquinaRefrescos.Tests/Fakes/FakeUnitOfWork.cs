using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Interfaces;

namespace MaquinaRefrescos.Tests.Fakes;

public class FakeProductoRepository : IProductoRepository
{
    public List<Producto> Items { get; }

    public FakeProductoRepository(IEnumerable<Producto>? iniciales = null)
    {
        Items = iniciales?.ToList() ?? new List<Producto>
        {
            new() { Id = 1, Nombre = "Coca-Cola", Precio = 17.00m, Existencia = 10 },
            new() { Id = 2, Nombre = "Fanta", Precio = 15.00m, Existencia = 10 },
            new() { Id = 3, Nombre = "Pepsi", Precio = 16.00m, Existencia = 10 },
            new() { Id = 4, Nombre = "Yoli", Precio = 14.00m, Existencia = 10 },
            new() { Id = 5, Nombre = "Coca-Cola Zero", Precio = 18.00m, Existencia = 0 }
        };
    }

    public Task<IReadOnlyList<Producto>> ObtenerTodosAsync()
        => Task.FromResult<IReadOnlyList<Producto>>(Items.OrderBy(p => p.Id).ToList());

    public Task<Producto?> ObtenerPorIdAsync(int id)
        => Task.FromResult(Items.FirstOrDefault(p => p.Id == id));
}

public class FakeMonederoRepository : IMonederoRepository
{
    public List<Moneda> Items { get; }

    public FakeMonederoRepository(IDictionary<decimal, int>? iniciales = null)
    {
        if (iniciales is not null)
        {
            Items = iniciales
                .Select(kv => new Moneda { Denominacion = kv.Key, Cantidad = kv.Value })
                .OrderBy(m => m.Denominacion)
                .ToList();
        }
        else
        {
            Items = new List<Moneda>
            {
                new() { Denominacion = 1.00m, Cantidad = 5 },
                new() { Denominacion = 2.00m, Cantidad = 5 },
                new() { Denominacion = 5.00m, Cantidad = 5 },
                new() { Denominacion = 10.00m, Cantidad = 5 }
            };
        }
    }

    public Task<IReadOnlyList<Moneda>> ObtenerTodasAsync()
        => Task.FromResult<IReadOnlyList<Moneda>>(Items.OrderBy(m => m.Denominacion).ToList());
}

public class FakeVentaRepository : IVentaRepository
{
    public List<Venta> Items { get; } = new();

    public Task<IReadOnlyList<Venta>> ObtenerHistorialAsync()
        => Task.FromResult<IReadOnlyList<Venta>>(Items.OrderByDescending(v => v.Fecha).ToList());

    public Task AgregarAsync(Venta venta)
    {
        venta.Id = Items.Count + 1;
        Items.Add(venta);
        return Task.CompletedTask;
    }
}

public class FakeUnitOfWork : IUnitOfWork
{
    public FakeProductoRepository FakeProductos { get; }
    public FakeMonederoRepository FakeMonedero { get; }
    public FakeVentaRepository FakeVentas { get; }
    public int VecesGuardado { get; private set; }

    public FakeUnitOfWork(
        IEnumerable<Producto>? productos = null,
        IDictionary<decimal, int>? monedero = null)
    {
        FakeProductos = new FakeProductoRepository(productos);
        FakeMonedero = new FakeMonederoRepository(monedero);
        FakeVentas = new FakeVentaRepository();
    }

    public IProductoRepository Productos => FakeProductos;
    public IMonederoRepository Monedero => FakeMonedero;
    public IVentaRepository Ventas => FakeVentas;

    public Task<int> GuardarAsync()
    {
        VecesGuardado++;
        return Task.FromResult(1);
    }
}
