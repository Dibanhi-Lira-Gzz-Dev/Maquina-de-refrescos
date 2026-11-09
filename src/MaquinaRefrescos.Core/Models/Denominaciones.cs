namespace MaquinaRefrescos.Core.Models;

public static class Denominaciones
{
    public static readonly IReadOnlyList<decimal> Permitidas = new decimal[] { 1m, 2m, 5m, 10m };

    public static bool EsValida(decimal valor) => Permitidas.Contains(valor);
}
