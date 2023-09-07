using Domain.Interfaces;
using Infrastructure.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace API.Installers;

public static class DatabaseInstaller
{
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryCache, MemoryCache>();
        services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
    }
}