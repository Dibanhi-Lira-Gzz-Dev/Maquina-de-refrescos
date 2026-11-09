namespace MaquinaRefrescos.Core.Interfaces;

public interface ICalculadoraCambio
{
    List<decimal>? Calcular(decimal monto, IDictionary<decimal, int> disponibles);
}
