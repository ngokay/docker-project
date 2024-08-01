using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Consumer.RabbitMQ
{
    public class WorkerBackgroundService : BackgroundService
    {
        private readonly ConnectionRabbitMQHepperExtension _rabbitMQClientServiceExtension;
        private readonly ILogger<WorkerBackgroundService> _logger;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public WorkerBackgroundService(ILogger<WorkerBackgroundService> logger,
            ConnectionRabbitMQHepperExtension _rabbitMQClientServiceExtension, IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            this._rabbitMQClientServiceExtension = _rabbitMQClientServiceExtension;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientServiceExtension.Connect();

            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //ConnectionRabbitMQHepperExtension.QueueName.Keys.ToList().ForEach(key =>
            //{
            //    var queueName = ConnectionRabbitMQHepperExtension.QueueName[key].ToList();
            //    queueName.ForEach(queue =>
            //    {
            //        string queueNameKey = queue.GetType().GetProperty("QueueName").GetValue(queue, null);
            //        var consumer = new AsyncEventingBasicConsumer(_channel);
            //        _channel.BasicConsume(queueNameKey, false, consumer);
            //        consumer.Received += Consumer_Received;
            //    });
            //});

            ConnectionRabbitMQHepperExtension.QueueName.Keys.ToList().ForEach(key =>
            {
                var queueName = ConnectionRabbitMQHepperExtension.QueueName[key].ToList();
                queueName.ForEach(queue =>
                {
                    string queueNameKey = queue.GetType().GetProperty("QueueName").GetValue(queue, null);

                    for (int i = 1; i < 5; i++)
                    {
                        var consumer = new AsyncEventingBasicConsumer(_channel);

                        consumer.Received += async (ch, ea) =>
                        {
                            var weatherForecast = JsonSerializer.Deserialize<dynamic>(Encoding.UTF8.GetString(ea.Body.ToArray()));

                            Console.WriteLine("\nNew Weather Forecast Notification!!!");
                            Console.WriteLine("\nConsumer : " + ea.ConsumerTag);
                            Console.WriteLine("\nRoutingKey : " + ea.RoutingKey);
                            Console.WriteLine("\nExchange : " + ea.Exchange);
                            Console.WriteLine(JsonSerializer.Serialize(weatherForecast));

                            _channel.BasicAck(ea.DeliveryTag, false);
                            await Task.Yield();

                        };
                        // this consumer tag identifies the subscription
                        // when it has to be cancelled
                        _channel.BasicConsume(queueNameKey, false, consumer);
                    }

                });
            });


        }

        //private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        //{
        //    var weatherForecast = JsonSerializer.Deserialize<dynamic>(Encoding.UTF8.GetString(@event.Body.ToArray()));

        //    Console.WriteLine("\nNew Weather Forecast Notification!!!");
        //    Console.WriteLine("\nConsumer : " + @event.ConsumerTag);
        //    Console.WriteLine("\nRoutingKey : " + @event.RoutingKey);
        //    Console.WriteLine("\nExchange : " + @event.Exchange);
        //    Console.WriteLine(JsonSerializer.Serialize(weatherForecast));

        //    _channel.BasicAck(@event.DeliveryTag, false);
        //}

    }
}
