namespace MaquinaRefrescos.Core.Interfaces;

public interface IUnitOfWork
{
    IProductoRepository Productos { get; }
    IMonederoRepository Monedero { get; }
    IVentaRepository Ventas { get; }

    Task<int> GuardarAsync();
}
