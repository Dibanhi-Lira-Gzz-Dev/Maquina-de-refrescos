using MaquinaRefrescos.Core.Interfaces;

namespace MaquinaRefrescos.Core.Services;

public class CalculadoraCambio : ICalculadoraCambio
{
    public List<decimal>? Calcular(decimal monto, IDictionary<decimal, int> disponibles)
    {
        if (monto < 0m)
        {
            return null;
        }

        if (monto == 0m)
        {
            return new List<decimal>();
        }

        var denominaciones = disponibles.Keys
            .Where(d => d > 0m)
            .OrderByDescending(d => d)
            .ToArray();

        var resultado = new List<decimal>();
        bool encontrado = BuscarCombinacion(monto, 0, denominaciones, disponibles, resultado);

        return encontrado ? resultado : null;
    }

    private static bool BuscarCombinacion(
        decimal restante,
        int indice,
        decimal[] denominaciones,
        IDictionary<decimal, int> disponibles,
        List<decimal> acumulado)
    {
        if (restante == 0m)
        {
            return true;
        }

        if (indice >= denominaciones.Length)
        {
            return false;
        }

        decimal moneda = denominaciones[indice];
        int disponiblesMoneda = disponibles.TryGetValue(moneda, out int cant) ? Math.Max(0, cant) : 0;
        int maxUsar = Math.Min((int)Math.Floor(restante / moneda), disponiblesMoneda);

        for (int n = maxUsar; n >= 0; n--)
        {
            for (int k = 0; k < n; k++)
            {
                acumulado.Add(moneda);
            }

            if (BuscarCombinacion(restante - (n * moneda), indice + 1, denominaciones, disponibles, acumulado))
            {
                return true;
            }

            if (n > 0)
            {
                acumulado.RemoveRange(acumulado.Count - n, n);
            }
        }

        return false;
    }
}
