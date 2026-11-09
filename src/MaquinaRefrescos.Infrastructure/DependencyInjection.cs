using MaquinaRefrescos.Core.Interfaces;
using MaquinaRefrescos.Core.Services;
using MaquinaRefrescos.Infrastructure.Data;
using MaquinaRefrescos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaquinaRefrescos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'Default'.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IMonederoRepository, MonederoRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICalculadoraCambio, CalculadoraCambio>();
        services.AddScoped<IMaquinaService, MaquinaService>();

        return services;
    }
}
