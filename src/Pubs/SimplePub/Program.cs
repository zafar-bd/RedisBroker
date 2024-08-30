using StackExchange.Redis;
using System.Text.Json;

namespace SimplePub
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string connectionString = "localhost";
            RedisChannel channelName = RedisChannel.Literal("test_channel");

            var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
            var subscriber = connection.GetSubscriber(channelName);

            var message = new RedisMessage(Guid.NewGuid(), "hellow world", DateTime.Now);
            var json = JsonSerializer.Serialize(message);

            await subscriber.PublishAsync(channelName, json);

            Console.ReadKey();
        }

        record RedisMessage(Guid Id, string Message, DateTime SentAt);
    }
}
