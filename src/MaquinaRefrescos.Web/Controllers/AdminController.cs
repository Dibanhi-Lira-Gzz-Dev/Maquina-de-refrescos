using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MaquinaRefrescos.Web.Controllers;

public class AdminController : Controller
{
    private readonly IMaquinaService _maquinaService;

    public AdminController(IMaquinaService maquinaService)
    {
        _maquinaService = maquinaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new AdminViewModel
        {
            Productos = await _maquinaService.ObtenerProductosAsync(),
            Monedero = await _maquinaService.ObtenerMonederoAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarExistencia(int productoId, int existencia)
    {
        if (existencia < 0)
        {
            TempData["MensajeAdmin"] = "La existencia no puede ser negativa.";
            return RedirectToAction(nameof(Index));
        }

        await _maquinaService.ActualizarInventarioAsync(productoId, existencia);
        TempData["MensajeAdmin"] = "Inventario actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarMonedero(int cant1, int cant2, int cant5, int cant10)
    {
        var nuevasCantidades = new Dictionary<decimal, int>
        {
            [1m] = Math.Max(0, cant1),
            [2m] = Math.Max(0, cant2),
            [5m] = Math.Max(0, cant5),
            [10m] = Math.Max(0, cant10)
        };

        await _maquinaService.ActualizarMonederoAsync(nuevasCantidades);
        TempData["MensajeAdmin"] = "Monedero actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VaciarMonedero()
    {
        var vacio = new Dictionary<decimal, int>
        {
            [1m] = 0,
            [2m] = 0,
            [5m] = 0,
            [10m] = 0
        };

        await _maquinaService.ActualizarMonederoAsync(vacio);
        TempData["MensajeAdmin"] = "Se dejó el monedero en 0 monedas (útil para probar compra sin cambio).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RestaurarMonedero()
    {
        var inicial = new Dictionary<decimal, int>
        {
            [1m] = 5,
            [2m] = 5,
            [5m] = 5,
            [10m] = 5
        };

        await _maquinaService.ActualizarMonederoAsync(inicial);
        TempData["MensajeAdmin"] = "Monedero restaurado a 5 monedas por denominación.";
        return RedirectToAction(nameof(Index));
    }
}
