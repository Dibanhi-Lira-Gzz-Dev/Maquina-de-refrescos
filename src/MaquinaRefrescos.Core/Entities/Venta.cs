namespace MaquinaRefrescos.Core.Entities;

public class Venta
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public decimal Precio { get; set; }
    public decimal MontoIngresado { get; set; }
    public decimal Cambio { get; set; }
    public string MonedasIngresadas { get; set; } = string.Empty;
    public string MonedasCambio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;

    public Producto? Producto { get; set; }
}
