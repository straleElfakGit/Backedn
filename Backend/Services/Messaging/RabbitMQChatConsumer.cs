using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Backend.Domain;

namespace Srbopoly.Services.Messaging;

public class RabbitMqChatConsumer : BackgroundService
{
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly IChannel _channel;

    public RabbitMqChatConsumer(IRabbitMqConnection rabbitMqConnection)
    {
        _rabbitMqConnection = rabbitMqConnection;
        _channel = _rabbitMqConnection.GetChannel();
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

            Console.WriteLine($"{message?.Username}: {message?.Text}");

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