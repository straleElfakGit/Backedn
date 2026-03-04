using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Backend.Domain;

namespace Srbopoly.Services.Messaging;

public interface IChatPublisher
{
    Task PublishAsync(int gameId, ChatMessage message);
}

public class RabbitMqChatPublisher : IChatPublisher
{
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public RabbitMqChatPublisher(IRabbitMqConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
    }

    public async Task PublishAsync(int gameId, ChatMessage message)
    {

        var channel = await _rabbitMqConnection
            .GetConnection()
            .CreateChannelAsync();


        var routingKey = $"game.{gameId.ToString()}";

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(message));

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: "game_chat_exchange",
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body);
        
        await channel.CloseAsync();
    }
} 