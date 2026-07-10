using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DarkPortal.Services;

public class MessageCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
                await messageService.CleanupOldMessages();
            }
            catch { }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}