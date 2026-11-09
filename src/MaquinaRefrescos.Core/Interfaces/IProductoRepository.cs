using MaquinaRefrescos.Core.Entities;

namespace MaquinaRefrescos.Core.Interfaces;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> ObtenerTodosAsync();
    Task<Producto?> ObtenerPorIdAsync(int id);
}
