using MaquinaRefrescos.Core.Entities;

namespace MaquinaRefrescos.Web.ViewModels;

public class AdminViewModel
{
    public IReadOnlyList<Producto> Productos { get; init; } = Array.Empty<Producto>();
    public IReadOnlyList<Moneda> Monedero { get; init; } = Array.Empty<Moneda>();
}
