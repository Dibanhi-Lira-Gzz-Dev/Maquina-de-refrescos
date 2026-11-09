using MaquinaRefrescos.Core.Services;

namespace MaquinaRefrescos.Tests;

public class CalculadoraCambioTests
{
    private readonly CalculadoraCambio _calculadora = new();

    [Fact]
    public void Calcular_MontoCero_DevuelveListaVacia()
    {
        var disponibles = new Dictionary<decimal, int>
        {
            [1m] = 5, [2m] = 5, [5m] = 5, [10m] = 5
        };

        var resultado = _calculadora.Calcular(0m, disponibles);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public void Calcular_CambioDe5_DevuelveUnaMonedaDe5()
    {
        var disponibles = new Dictionary<decimal, int>
        {
            [1m] = 5, [2m] = 5, [5m] = 5, [10m] = 5
        };

        var resultado = _calculadora.Calcular(5m, disponibles);

        Assert.NotNull(resultado);
        Assert.Equal(new List<decimal> { 5m }, resultado);
    }

    [Fact]
    public void Calcular_CuandoVorazFalla_EncuentraCombinacionConBacktracking()
    {
        var disponibles = new Dictionary<decimal, int>
        {
            [1m] = 0,
            [2m] = 3,
            [5m] = 1,
            [10m] = 0
        };

        var resultado = _calculadora.Calcular(6m, disponibles);

        Assert.NotNull(resultado);
        Assert.Equal(new List<decimal> { 2m, 2m, 2m }, resultado);
    }

    [Fact]
    public void Calcular_SinMonedasSuficientes_DevuelveNull()
    {
        var disponibles = new Dictionary<decimal, int>
        {
            [1m] = 0,
            [2m] = 1,
            [5m] = 0,
            [10m] = 2
        };

        var resultado = _calculadora.Calcular(5m, disponibles);

        Assert.Null(resultado);
    }
}
