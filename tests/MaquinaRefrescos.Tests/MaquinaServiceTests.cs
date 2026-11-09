using MaquinaRefrescos.Core.Models;
using MaquinaRefrescos.Core.Services;
using MaquinaRefrescos.Tests.Fakes;

namespace MaquinaRefrescos.Tests;

public class MaquinaServiceTests
{
    [Fact]
    public async Task Caso1_CompraExactaDeCocaCola17_EntregaProductoSinCambio()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var resultado = await service.ComprarAsync(1, new List<decimal> { 10m, 5m, 2m });

        Assert.Equal(EstadoCompra.Exitosa, resultado.Estado);
        Assert.Equal("Coca-Cola", resultado.ProductoEntregado);
        Assert.Equal(17m, resultado.TotalIngresado);
        Assert.Empty(resultado.Cambio);
        Assert.Equal(0m, resultado.TotalCambio);

        var coca = await uow.Productos.ObtenerPorIdAsync(1);
        Assert.Equal(9, coca!.Existencia);
        Assert.Single(uow.FakeVentas.Items);
    }

    [Fact]
    public async Task Caso2_CompraDeFanta15Pagando20_Recibe5DeCambio()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var resultado = await service.ComprarAsync(2, new List<decimal> { 10m, 10m });

        Assert.Equal(EstadoCompra.Exitosa, resultado.Estado);
        Assert.Equal("Fanta", resultado.ProductoEntregado);
        Assert.Equal(20m, resultado.TotalIngresado);
        Assert.Equal(5m, resultado.TotalCambio);
        Assert.Equal(new List<decimal> { 5m }, resultado.Cambio);

        var monedero = await uow.Monedero.ObtenerTodasAsync();
        Assert.Equal(7, monedero.First(m => m.Denominacion == 10m).Cantidad);
        Assert.Equal(4, monedero.First(m => m.Denominacion == 5m).Cantidad);
    }

    [Fact]
    public async Task Caso3_IntentoDeCompraSinInventario_CancelaYDevuelveMonedas()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var monedas = new List<decimal> { 10m, 5m, 2m, 1m };
        var resultado = await service.ComprarAsync(5, monedas);

        Assert.Equal(EstadoCompra.SinInventario, resultado.Estado);
        Assert.Equal(monedas, resultado.MonedasDevueltas);
        Assert.Empty(uow.FakeVentas.Items);
        Assert.Equal(0, uow.VecesGuardado);
    }

    [Fact]
    public async Task Caso4_CuandoNoExisteCambioDisponible_CancelaYRegresaTodasLasMonedas()
    {
        var monederoVacio = new Dictionary<decimal, int>
        {
            [1m] = 0, [2m] = 0, [5m] = 0, [10m] = 0
        };
        var uow = new FakeUnitOfWork(monedero: monederoVacio);
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var monedasIngresadas = new List<decimal> { 10m, 10m };
        var resultado = await service.ComprarAsync(2, monedasIngresadas);

        Assert.Equal(EstadoCompra.SinCambio, resultado.Estado);
        Assert.Equal(new List<decimal> { 10m, 10m }, resultado.MonedasDevueltas);
        Assert.Empty(resultado.Cambio);

        var fanta = await uow.Productos.ObtenerPorIdAsync(2);
        Assert.Equal(10, fanta!.Existencia);
        Assert.All(uow.FakeMonedero.Items, m => Assert.Equal(0, m.Cantidad));
        Assert.Empty(uow.FakeVentas.Items);
        Assert.Equal(0, uow.VecesGuardado);
    }

    [Fact]
    public async Task Caso5_CompraConCambioExactoDe2_EntregaCambioDe2()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var resultado = await service.ComprarAsync(4, new List<decimal> { 10m, 5m, 1m });

        Assert.Equal(EstadoCompra.Exitosa, resultado.Estado);
        Assert.Equal("Yoli", resultado.ProductoEntregado);
        Assert.Equal(2m, resultado.TotalCambio);
        Assert.Equal(new List<decimal> { 2m }, resultado.Cambio);
    }

    [Fact]
    public async Task Comprar_CuandoElDineroEsInsuficiente_IndicaMontoFaltante()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var resultado = await service.ComprarAsync(3, new List<decimal> { 10m });

        Assert.Equal(EstadoCompra.DineroInsuficiente, resultado.Estado);
        Assert.Equal(10m, resultado.TotalIngresado);
        Assert.Equal(6m, resultado.Faltante);
        Assert.Empty(uow.FakeVentas.Items);
    }

    [Fact]
    public async Task Comprar_MonedaNoPermitida_RechazaYDevuelveMonedas()
    {
        var uow = new FakeUnitOfWork();
        var service = new MaquinaService(uow, new CalculadoraCambio());

        var resultado = await service.ComprarAsync(1, new List<decimal> { 20m });

        Assert.Equal(EstadoCompra.MonedaInvalida, resultado.Estado);
        Assert.Equal(new List<decimal> { 20m }, resultado.MonedasDevueltas);
        Assert.Empty(uow.FakeVentas.Items);
    }
}
