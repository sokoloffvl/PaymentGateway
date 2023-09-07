using Domain.Models;
using Infrastructure.EventBus;

namespace API.Installers;

public static class MessageBusInstaller
{
    public static void AddMessageBus(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundQueue<Payment>>(s => new ConflatingQueue<Payment,string>(p => p.PaymentId));
        services.AddHostedService<BackgroundRequestProcessor>();
    }
}