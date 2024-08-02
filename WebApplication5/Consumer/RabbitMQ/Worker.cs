using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Consumer.RabbitMQ
{
    public class Worker : BackgroundService
    {
        private readonly ConnectionRabbitMQHepper _rabbitMQClientService;
        private readonly ILogger<Worker> _logger;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, ConnectionRabbitMQHepper rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false); // đảm bảo xử lý 1 tin nhắn xong mới gửi tiếp tin nhắn vào consumer
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for(int i = 1; i < 5; i++)
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                _channel.BasicConsume(ConnectionRabbitMQHepper.QueueName, false, consumer);
                consumer.Received += Consumer_Received;
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var weatherForecast = JsonSerializer.Deserialize<dynamic>(Encoding.UTF8.GetString(@event.Body.ToArray()));

            Console.WriteLine("\nNew Weather Forecast Notification!!!");
            Console.WriteLine("\nConsumer"+ @event.ConsumerTag);
            Console.WriteLine(JsonSerializer.Serialize(weatherForecast));

            _channel.BasicAck(@event.DeliveryTag, false);
        }

    }
}
