using System.Globalization;
using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Core.Models;
using MaquinaRefrescos.Web.Helpers;
using MaquinaRefrescos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MaquinaRefrescos.Web.Controllers;

public class MaquinaController : Controller
{
    private readonly IMaquinaService _maquinaService;

    public MaquinaController(IMaquinaService maquinaService)
    {
        _maquinaService = maquinaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var productos = await _maquinaService.ObtenerProductosAsync();

        int? productoId = SesionCompra.ObtenerProductoId(HttpContext.Session);
        var seleccionado = productoId.HasValue
            ? productos.FirstOrDefault(p => p.Id == productoId.Value)
            : null;

        var viewModel = new MaquinaViewModel
        {
            Productos = productos,
            ProductoSeleccionado = seleccionado,
            MonedasIngresadas = SesionCompra.ObtenerMonedas(HttpContext.Session)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Seleccionar(int productoId)
    {
        var producto = await _maquinaService.ObtenerProductoPorIdAsync(productoId);
        if (producto is null)
        {
            GuardarAlerta("danger", "El producto seleccionado no existe.");
            return RedirectToAction(nameof(Index));
        }

        SesionCompra.GuardarProductoId(HttpContext.Session, productoId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult InsertarMoneda(decimal valor)
    {
        if (!Denominaciones.EsValida(valor))
        {
            GuardarAlerta("danger", "Moneda no permitida. Solo se aceptan $1, $2, $5 y $10.");
            return RedirectToAction(nameof(Index));
        }

        SesionCompra.AgregarMoneda(HttpContext.Session, valor);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comprar()
    {
        int? productoId = SesionCompra.ObtenerProductoId(HttpContext.Session);
        if (!productoId.HasValue)
        {
            GuardarAlerta("warning", "Primero selecciona un producto.");
            return RedirectToAction(nameof(Index));
        }

        var monedas = SesionCompra.ObtenerMonedas(HttpContext.Session);
        await EjecutarCompraAsync(productoId.Value, monedas);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar()
    {
        var monedas = SesionCompra.ObtenerMonedas(HttpContext.Session);
        SesionCompra.LimpiarTodo(HttpContext.Session);

        if (monedas.Count > 0)
        {
            decimal total = monedas.Sum();
            string detalle = FormatearMonedas(monedas);
            GuardarAlerta("secondary", $"Operación cancelada. Monedas devueltas: ${total:0.00} ({detalle}).");
        }
        else
        {
            GuardarAlerta("secondary", "Selección cancelada.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Historial()
    {
        var ventas = await _maquinaService.ObtenerHistorialAsync();
        return View(ventas);
    }

    private async Task EjecutarCompraAsync(int productoId, List<decimal> monedas)
    {
        var resultado = await _maquinaService.ComprarAsync(productoId, monedas);
        GuardarResultadoEnTempData(resultado);

        switch (resultado.Estado)
        {
            case EstadoCompra.Exitosa:
                SesionCompra.LimpiarTodo(HttpContext.Session);
                break;

            case EstadoCompra.SinCambio:
                SesionCompra.LimpiarMonedas(HttpContext.Session);
                break;

            case EstadoCompra.SinInventario:
            case EstadoCompra.ProductoNoExiste:
            case EstadoCompra.MonedaInvalida:
                SesionCompra.LimpiarTodo(HttpContext.Session);
                break;

            case EstadoCompra.DineroInsuficiente:
                break;
        }
    }

    private void GuardarResultadoEnTempData(ResultadoCompra resultado)
    {
        string tipo = resultado.Estado switch
        {
            EstadoCompra.Exitosa => "success",
            EstadoCompra.DineroInsuficiente => "warning",
            _ => "danger"
        };

        string mensaje = resultado.Mensaje;

        if (resultado.Cambio.Count > 0)
        {
            mensaje += $" Monedas de cambio: {FormatearMonedas(resultado.Cambio)}.";
        }

        if (resultado.MonedasDevueltas.Count > 0)
        {
            mensaje += $" Monedas devueltas: {FormatearMonedas(resultado.MonedasDevueltas)}.";
        }

        GuardarAlerta(tipo, mensaje);
    }

    private void GuardarAlerta(string tipo, string mensaje)
    {
        TempData["TipoAlerta"] = tipo;
        TempData["MensajeAlerta"] = mensaje;
    }

    private static string FormatearMonedas(IEnumerable<decimal> monedas)
        => string.Join(", ", monedas.Select(m => $"${m.ToString("0.##", CultureInfo.InvariantCulture)}"));
}
