using API.Services;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.BankCommunication;

namespace API.Installers;

public static class ServicesInstaller
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IMaskingService, MaskingService>();
        services.AddSingleton<IBankClient, BankClient>();
        services.AddSingleton<IPaymentProcessingService, PaymentProcessingService>();
    }
}