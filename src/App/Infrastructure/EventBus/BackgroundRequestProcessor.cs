using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EventBus;

public class BackgroundRequestProcessor : BackgroundService
{
    private readonly IBackgroundQueue<Payment> queue;
    private readonly IPaymentProcessingService paymentProcessingService;
    private readonly ILogger<BackgroundRequestProcessor> logger;

    public BackgroundRequestProcessor(IBackgroundQueue<Payment> queue,
        IPaymentProcessingService paymentProcessingService,
        ILogger<BackgroundRequestProcessor> logger)
    {
        this.queue = queue;
        this.paymentProcessingService = paymentProcessingService;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Payment paymentToProcess = null;
            try
            {
                if (queue.TryDequeue(out paymentToProcess))
                {
                    Task.Run(async () => await paymentProcessingService.Process(paymentToProcess.PaymentId));
                }
                else await Task.Delay(50, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing work item");
            }
        }
    }
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{nameof(BackgroundRequestProcessor)} is stopping.");
        await base.StopAsync(stoppingToken);
    }
}