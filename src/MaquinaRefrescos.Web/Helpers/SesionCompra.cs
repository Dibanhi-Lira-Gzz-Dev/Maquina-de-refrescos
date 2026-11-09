using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MaquinaRefrescos.Web.Helpers;

public static class SesionCompra
{
    private const string KeyProductoId = "Maquina_ProductoId";
    private const string KeyMonedas = "Maquina_Monedas";

    public static int? ObtenerProductoId(ISession session)
        => session.GetInt32(KeyProductoId);

    public static void GuardarProductoId(ISession session, int? productoId)
    {
        if (productoId.HasValue)
        {
            session.SetInt32(KeyProductoId, productoId.Value);
        }
        else
        {
            session.Remove(KeyProductoId);
        }
    }

    public static List<decimal> ObtenerMonedas(ISession session)
    {
        string? json = session.GetString(KeyMonedas);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<decimal>();
        }

        return JsonSerializer.Deserialize<List<decimal>>(json) ?? new List<decimal>();
    }

    public static void AgregarMoneda(ISession session, decimal moneda)
    {
        var monedas = ObtenerMonedas(session);
        monedas.Add(moneda);
        session.SetString(KeyMonedas, JsonSerializer.Serialize(monedas));
    }

    public static void LimpiarMonedas(ISession session)
        => session.Remove(KeyMonedas);

    public static void LimpiarTodo(ISession session)
    {
        session.Remove(KeyProductoId);
        session.Remove(KeyMonedas);
    }
}
