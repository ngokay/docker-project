using RabbitMQ.Client;

namespace Consumer.RabbitMQ
{
    public class ConnectionRabbitMQHepper : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<ConnectionRabbitMQHepper> _logger;

        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "test-exchange";
        public static string RoutingWeatherForecast = "weather-forecast-route";
        public static string QueueName = "weather-forecast-queue";

        public ConnectionRabbitMQHepper(ILogger<ConnectionRabbitMQHepper> logger, ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        public IModel Connect()
        {
            if (_channel is { IsOpen: true })
            {
                return this._channel;
            }

            _channel = this.Connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Topic, true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingWeatherForecast);
            _logger.LogInformation("Connected RabbitMQ successfuly...");

            return _channel;
        }

        private IConnection Connection
        {
            get
            {
                if (_connection == null) // _connection defined in class -- private static IConnection _connection;
                {
                    _connection = CreateConnection();
                }
                return _connection;
            }
        }

        private IConnection CreateConnection()
        {
            // why do we need to set this explicitly? shouldn't this be the default?
            _connectionFactory.AutomaticRecoveryEnabled = true;

            // what is a good value to use?
            _connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

            IConnection connection = _connectionFactory.CreateConnection();
            return connection;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("Lost connection with RabbitMQ...");
        }
    }
}
