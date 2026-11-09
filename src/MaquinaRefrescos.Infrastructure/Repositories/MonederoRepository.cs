using MaquinaRefrescos.Core.Entities;
using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaquinaRefrescos.Infrastructure.Repositories;

public class MonederoRepository : IMonederoRepository
{
    private readonly AppDbContext _context;

    public MonederoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Moneda>> ObtenerTodasAsync()
        => await _context.Monedero.OrderBy(m => m.Denominacion).ToListAsync();
}
