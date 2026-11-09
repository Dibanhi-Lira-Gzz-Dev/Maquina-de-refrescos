using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Models;

namespace MaquinaRefrescos.Core.Interfaces;

public interface IMaquinaService
{
    Task<IReadOnlyList<Producto>> ObtenerProductosAsync();
    Task<Producto?> ObtenerProductoPorIdAsync(int id);
    Task<IReadOnlyList<Moneda>> ObtenerMonederoAsync();
    Task<IReadOnlyList<Venta>> ObtenerHistorialAsync();
    Task<ResultadoCompra> ComprarAsync(int productoId, IReadOnlyList<decimal> monedasIngresadas);
    Task ActualizarInventarioAsync(int productoId, int nuevaExistencia);
    Task ActualizarMonederoAsync(IDictionary<decimal, int> cantidades);
}
