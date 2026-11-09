namespace MaquinaRefrescos.Core.Models;

public class ResultadoCompra
{
    public EstadoCompra Estado { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    public string? ProductoEntregado { get; init; }
    public decimal TotalIngresado { get; init; }
    public decimal Faltante { get; init; }
    public decimal TotalCambio => Cambio.Sum();
    public List<decimal> Cambio { get; init; } = new();
    public List<decimal> MonedasDevueltas { get; init; } = new();
}
