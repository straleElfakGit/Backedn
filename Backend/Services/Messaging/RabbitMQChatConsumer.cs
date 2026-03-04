using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Backend.Domain;
using Microsoft.AspNetCore.SignalR;

namespace Srbopoly.Services.Messaging;

public class RabbitMqChatConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly IChannel _channel;
    private readonly IHubContext<ChatHub.ChatHub> _chatHub;

    public RabbitMqChatConsumer(IRabbitMqConnection rabbitMqConnection,
     IHubContext<ChatHub.ChatHub> chatHub)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _channel = _rabbitMqConnection.GetChannel();
         _chatHub = chatHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueName = "chat_backend_queue";

        await _channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: "game_chat_exchange",
            routingKey: "game.*");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            var body = args.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<ChatMessage>(json);

             if (message != null)
            {
                await _chatHub.Clients.Group($"game-{message.GameId}")
                                      .SendAsync("ReceiveMessage", message);
            }

           //Console.WriteLine($"{message?.Username}: {message?.Text}");

            await _channel.BasicAckAsync(args.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
        //return Task.CompletedTask;
    }
}