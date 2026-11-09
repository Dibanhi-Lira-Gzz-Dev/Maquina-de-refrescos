using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MaquinaRefrescos.Web.Controllers.Api;

[ApiController]
[Route("api/maquina")]
public class MaquinaApiController : ControllerBase
{
    private readonly IMaquinaService _maquinaService;

    public MaquinaApiController(IMaquinaService maquinaService)
    {
        _maquinaService = maquinaService;
    }

    [HttpGet("productos")]
    public async Task<IActionResult> ObtenerProductos()
    {
        var productos = await _maquinaService.ObtenerProductosAsync();
        return Ok(productos);
    }

    [HttpGet("monedero")]
    public async Task<IActionResult> ObtenerMonedero()
    {
        var monedero = await _maquinaService.ObtenerMonederoAsync();
        return Ok(monedero);
    }

    [HttpGet("ventas")]
    public async Task<IActionResult> ObtenerVentas()
    {
        var ventas = await _maquinaService.ObtenerHistorialAsync();
        return Ok(ventas);
    }

    [HttpPost("compras")]
    public async Task<IActionResult> Comprar([FromBody] CompraApiRequest request)
    {
        if (request is null)
        {
            return BadRequest(new { mensaje = "Solicitud inválida." });
        }

        var resultado = await _maquinaService.ComprarAsync(request.ProductoId, request.Monedas ?? new List<decimal>());

        return resultado.Estado == EstadoCompra.Exitosa
            ? Ok(resultado)
            : BadRequest(resultado);
    }
}

public class CompraApiRequest
{
    public int ProductoId { get; set; }
    public List<decimal> Monedas { get; set; } = new();
}
