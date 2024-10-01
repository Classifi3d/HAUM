using HAUM_BackEnd.Repositories;
using System.Threading;
namespace HAUM_BackEnd.WebSockets
{
    public class WebSocketBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _retryIntervalSeconds = 60;

        public WebSocketBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Crated a new WebSocketBackgroundServiceScope");

                // Create a new scope to obtain scoped services
                using (var scope = _serviceProvider.CreateScope())
                {
                    var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    var webSocketManager = scope.ServiceProvider.GetRequiredService<WebSocketManager>();

                    var devices = await deviceRepository.GetAllDevicesAsync();
                    await webSocketManager.ConnectToWebSocketsAsync(devices,stoppingToken);
                }
                await Task.Delay(_retryIntervalSeconds * 1000, stoppingToken);
            }
        }
    }
}
