using MaquinaRefrescos.Core.Entities;

namespace MaquinaRefrescos.Core.Interfaces;

public interface IMonederoRepository
{
    Task<IReadOnlyList<Moneda>> ObtenerTodasAsync();
}
