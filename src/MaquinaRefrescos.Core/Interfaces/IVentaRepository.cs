using MaquinaRefrescos.Core.Entities;

namespace MaquinaRefrescos.Core.Interfaces;

public interface IVentaRepository
{
    Task<IReadOnlyList<Venta>> ObtenerHistorialAsync();
    Task AgregarAsync(Venta venta);
}
