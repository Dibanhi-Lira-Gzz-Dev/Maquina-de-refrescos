using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Models;

namespace MaquinaRefrescos.Web.ViewModels;

public class MaquinaViewModel
{
    public IReadOnlyList<Producto> Productos { get; init; } = Array.Empty<Producto>();
    public Producto? ProductoSeleccionado { get; init; }
    public List<decimal> MonedasIngresadas { get; init; } = new();
    public IReadOnlyList<decimal> DenominacionesPermitidas { get; init; } = Denominaciones.Permitidas;

    public decimal TotalIngresado => MonedasIngresadas.Sum();

    public decimal MontoFaltante => ProductoSeleccionado is null
        ? 0m
        : Math.Max(0m, ProductoSeleccionado.Precio - TotalIngresado);
}
