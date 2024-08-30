using StackExchange.Redis;
using System.Text.Json;

namespace SimpleSub
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                string connectionString = "localhost";
                RedisChannel channelName = RedisChannel.Literal("test_channel");

                var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
                var subscriber = connection.GetSubscriber(channelName);

                var me = await subscriber.SubscribeAsync(channelName);
                var m = await me.ReadAsync(stoppingToken);
                var data = JsonSerializer.Deserialize<RedisMessage>(m.Message);
                Console.WriteLine(data);
                //await subscriber.SubscribeAsync(channelName, (channelName, message) =>
                //{
                //    var m = JsonSerializer.Deserialize<RedisMessage>(message);

                //});
                //await Task.Delay(1000, stoppingToken);
            }
        }
    }

    record RedisMessage(Guid Id, string Message, DateTime SentAt);
}
