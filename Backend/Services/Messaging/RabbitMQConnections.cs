using RabbitMQ.Client;

namespace Srbopoly.Services.Messaging;

public interface IRabbitMqConnection
{
    IConnection GetConnection();
    IChannel GetChannel();
}

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        var connectionString = _configuration["RabbitMq:ConnectionString"];

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel =  await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(
            exchange: "game_chat_exchange",
            type: ExchangeType.Topic,
            durable: true);
    }

    public IConnection GetConnection() => _connection;

    public IChannel GetChannel() => _channel;

    public async ValueTask DisposeAsync()
    {
        if(_channel != null)
            await _channel.CloseAsync();
        if(_connection != null)
            await _connection.CloseAsync();
    }

    public void Dispose()
    {
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}