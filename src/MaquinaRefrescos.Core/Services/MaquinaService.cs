using System.Globalization;
using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Core.Models;

namespace MaquinaRefrescos.Core.Services;

public class MaquinaService : IMaquinaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalculadoraCambio _calculadoraCambio;

    public MaquinaService(IUnitOfWork unitOfWork, ICalculadoraCambio calculadoraCambio)
    {
        _unitOfWork = unitOfWork;
        _calculadoraCambio = calculadoraCambio;
    }

    public Task<IReadOnlyList<Producto>> ObtenerProductosAsync()
        => _unitOfWork.Productos.ObtenerTodosAsync();

    public Task<Producto?> ObtenerProductoPorIdAsync(int id)
        => _unitOfWork.Productos.ObtenerPorIdAsync(id);

    public Task<IReadOnlyList<Moneda>> ObtenerMonederoAsync()
        => _unitOfWork.Monedero.ObtenerTodasAsync();

    public Task<IReadOnlyList<Venta>> ObtenerHistorialAsync()
        => _unitOfWork.Ventas.ObtenerHistorialAsync();

    public async Task<ResultadoCompra> ComprarAsync(int productoId, IReadOnlyList<decimal> monedasIngresadas)
    {
        decimal totalIngresado = monedasIngresadas.Sum();

        if (monedasIngresadas.Any(m => !Denominaciones.EsValida(m)))
        {
            return new ResultadoCompra
            {
                Estado = EstadoCompra.MonedaInvalida,
                TotalIngresado = totalIngresado,
                MonedasDevueltas = monedasIngresadas.ToList(),
                Mensaje = "Solo se permiten monedas de $1, $2, $5 y $10."
            };
        }

        var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(productoId);
        if (producto is null)
        {
            return new ResultadoCompra
            {
                Estado = EstadoCompra.ProductoNoExiste,
                TotalIngresado = totalIngresado,
                MonedasDevueltas = monedasIngresadas.ToList(),
                Mensaje = "El producto seleccionado no existe."
            };
        }

        if (producto.Existencia <= 0)
        {
            return new ResultadoCompra
            {
                Estado = EstadoCompra.SinInventario,
                TotalIngresado = totalIngresado,
                MonedasDevueltas = monedasIngresadas.ToList(),
                Mensaje = $"No hay existencia de {producto.Nombre}. Se devuelven tus monedas."
            };
        }

        if (totalIngresado < producto.Precio)
        {
            decimal faltante = producto.Precio - totalIngresado;
            return new ResultadoCompra
            {
                Estado = EstadoCompra.DineroInsuficiente,
                TotalIngresado = totalIngresado,
                Faltante = faltante,
                Mensaje = $"Monto insuficiente para {producto.Nombre} (${producto.Precio:0.00}). Faltan ${faltante:0.00}."
            };
        }

        var monederoActual = await _unitOfWork.Monedero.ObtenerTodasAsync();
        var disponibles = Denominaciones.Permitidas.ToDictionary(d => d, _ => 0);

        foreach (var fila in monederoActual)
        {
            disponibles[fila.Denominacion] = fila.Cantidad;
        }

        foreach (decimal moneda in monedasIngresadas)
        {
            disponibles[moneda] = disponibles.GetValueOrDefault(moneda) + 1;
        }

        decimal cambioRequerido = totalIngresado - producto.Precio;
        var monedasCambio = _calculadoraCambio.Calcular(cambioRequerido, disponibles);

        if (monedasCambio is null)
        {
            return new ResultadoCompra
            {
                Estado = EstadoCompra.SinCambio,
                TotalIngresado = totalIngresado,
                MonedasDevueltas = monedasIngresadas.ToList(),
                Mensaje = $"No hay cambio disponible (${cambioRequerido:0.00}). Operación cancelada: se devuelven tus monedas (${totalIngresado:0.00}). Por favor ingresa nuevamente el dinero."
            };
        }

        foreach (decimal monedaEntregada in monedasCambio)
        {
            disponibles[monedaEntregada] -= 1;
        }

        foreach (var fila in monederoActual)
        {
            if (disponibles.TryGetValue(fila.Denominacion, out int nuevaCantidad))
            {
                fila.Cantidad = nuevaCantidad;
            }
        }

        producto.Existencia -= 1;

        var venta = new Venta
        {
            ProductoId = producto.Id,
            Precio = producto.Precio,
            MontoIngresado = totalIngresado,
            Cambio = cambioRequerido,
            MonedasIngresadas = FormatearListaMonedas(monedasIngresadas),
            MonedasCambio = FormatearListaMonedas(monedasCambio),
            Fecha = DateTime.Now
        };

        await _unitOfWork.Ventas.AgregarAsync(venta);
        await _unitOfWork.GuardarAsync();

        string mensajeExito = cambioRequerido == 0m
            ? $"Producto entregado: {producto.Nombre}. Pago exacto, sin cambio."
            : $"Producto entregado: {producto.Nombre}. Cambio entregado: ${cambioRequerido:0.00}.";

        return new ResultadoCompra
        {
            Estado = EstadoCompra.Exitosa,
            ProductoEntregado = producto.Nombre,
            TotalIngresado = totalIngresado,
            Cambio = monedasCambio,
            Mensaje = mensajeExito
        };
    }

    public async Task ActualizarInventarioAsync(int productoId, int nuevaExistencia)
    {
        if (nuevaExistencia < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nuevaExistencia), "La existencia no puede ser negativa.");
        }

        var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(productoId)
            ?? throw new InvalidOperationException("Producto no encontrado.");

        producto.Existencia = nuevaExistencia;
        await _unitOfWork.GuardarAsync();
    }

    public async Task ActualizarMonederoAsync(IDictionary<decimal, int> cantidades)
    {
        var monederoActual = await _unitOfWork.Monedero.ObtenerTodasAsync();
        foreach (var fila in monederoActual)
        {
            if (cantidades.TryGetValue(fila.Denominacion, out int cantidad) && cantidad >= 0)
            {
                fila.Cantidad = cantidad;
            }
        }

        await _unitOfWork.GuardarAsync();
    }

    private static string FormatearListaMonedas(IEnumerable<decimal> monedas)
        => string.Join(",", monedas.Select(m => m.ToString("0.##", CultureInfo.InvariantCulture)));
}
