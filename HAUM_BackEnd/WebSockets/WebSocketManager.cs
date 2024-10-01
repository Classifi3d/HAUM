using HAUM_BackEnd.Context;
using HAUM_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace HAUM_BackEnd.WebSockets
{
    public class WebSocketManager
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _retryIntervalSeconds;

        public WebSocketManager(
            IServiceScopeFactory scopeFactory,
            int retryIntervalSeconds = 30)
        {
            _scopeFactory = scopeFactory;
            _retryIntervalSeconds = retryIntervalSeconds;
        }

        public async Task ConnectToWebSocketsAsync(IEnumerable<Device> devices, CancellationToken stoppingToken)
        {
            var tasks = devices.Select(device => ConnectToWebSocketWithRetryAsync(device, stoppingToken));
            await Task.WhenAll(tasks);
        }

        private async Task ConnectToWebSocketWithRetryAsync(Device device, CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ConnectToWebSocketAsync(device, stoppingToken);
                    return; // Exit the loop if the connection is successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to {device.IpAddress}:{device.Port}. Retrying in {_retryIntervalSeconds} seconds. Error: {ex.Message}");
                    await Task.Delay(_retryIntervalSeconds * 1000, stoppingToken);
                }
            }
        }

        private async Task ConnectToWebSocketAsync(Device device, CancellationToken stoppingToken)
        {
            var serverUri = new Uri($"ws://{device.IpAddress}:{device.Port}");
            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                await clientWebSocket.ConnectAsync(serverUri, stoppingToken);
                Console.WriteLine($"Opened connection to {serverUri}");

                await ReceiveMessages(clientWebSocket, device, stoppingToken);

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", stoppingToken);
            }
        }

        async Task ReceiveMessages(ClientWebSocket webSocket, Device device, CancellationToken stoppingToken)
        {
            byte[] buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", stoppingToken);
                    Console.WriteLine("Closed by the server.");
                }
                else
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {message}");
                    try
                    {
                        JsonData jsonData = JsonSerializer.Deserialize<JsonData>(message);
                        await SaveSensorData(device, jsonData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Wrong Data Sent!");
                    }
                }
            }
        }

        async Task SaveSensorData(Device device, JsonData jsonData)
        {

            DateTime dateTime = DateTime.UtcNow;
            using (var scope = _scopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Data data = new Data()
                {
                    Time = dateTime,
                    Device = device
                };
                try
                {
                    if (jsonData.Temperature != null)
                    {

                        float dataNumValue = (float)Convert.ToDouble(jsonData.Temperature);
                        data.DataValue = dataNumValue;
                        data.Type = SensorTypeEnum.Temperature;
                        device.Datas!.Add(data);
                        _dbContext.Device.Update(device);
                        await _dbContext.Data.AddAsync(data);
                        await _dbContext.SaveChangesAsync();

                    }
                    if (jsonData.Humidity != null)
                    {

                        float dataNumValue = (float)Convert.ToDouble(jsonData.Humidity);
                        data.DataValue = dataNumValue;
                        data.Type = SensorTypeEnum.Humidity;
                        device.Datas!.Add(data);
                        _dbContext.Device.Update(device);
                        await _dbContext.Data.AddAsync(data);
                        await _dbContext.SaveChangesAsync();

                    }
                    if (jsonData.Pressure != null)
                    {

                        float dataNumValue = (float)Convert.ToDouble(jsonData.Pressure);
                        data.DataValue = dataNumValue;
                        data.Type = SensorTypeEnum.Pressure;
                        device.Datas!.Add(data);
                        _dbContext.Device.Update(device);
                        await _dbContext.Data.AddAsync(data);
                        await _dbContext.SaveChangesAsync();

                    }
                    if (jsonData.Illumination != null)
                    {

                        float dataNumValue = (float)Convert.ToDouble(jsonData.Illumination);
                        data.DataValue = dataNumValue;
                        data.Type = SensorTypeEnum.Illumination;
                        device.Datas!.Add(data);
                        _dbContext.Device.Update(device);
                        await _dbContext.Data.AddAsync(data);
                        await _dbContext.SaveChangesAsync();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
