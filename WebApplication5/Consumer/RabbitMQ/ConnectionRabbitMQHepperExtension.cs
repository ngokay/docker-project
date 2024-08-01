using RabbitMQ.Client;

namespace Consumer.RabbitMQ
{
    public class ConnectionRabbitMQHepperExtension : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly ILogger<ConnectionRabbitMQHepperExtension> _logger;

        private IConnection _connection;
        private IModel _channel;


        //public static string ExchangeName = "test-exchange";
        //public static string RoutingWeatherForecast = "weather-forecast-route";
        //public static string QueueName = "weather-forecast-queue";

        public static Dictionary<string, IList<dynamic>> QueueName = new Dictionary<string, IList<dynamic>>()
        {
            ["Direct"] = new List<dynamic>() { 
                new { QueueName = "Direct-QueueName-1" }, 
                new { QueueName = "Direct-QueueName-2" } 
            },
            ["Fanout"] = new List<dynamic>() { 
                new { QueueName = "Fanout-QueueName-1" }, 
                new { QueueName = "Fanout-QueueName-2" }, 
                new { QueueName = "Fanout-QueueName-3" }, 
                new { QueueName = "Fanout-QueueName-4" }, 
            },
            ["Topic"] = new List<dynamic>() { 
                new { QueueName = "Topic-QueueName-1" },
                new { QueueName = "Topic-All-QueueName-1" },
                new { QueueName = "Topic-QueueName-2" } ,
                new { QueueName = "Topic-All-QueueName-2" } ,

            },
            ["Headers"] = new List<dynamic>() { 
                new { QueueName = "Headers-QueueName-1" }, 
                new { QueueName = "Headers-QueueName-2" } 
            },
        };


        public static Dictionary<string, IList<dynamic>> ExchangeName = new Dictionary<string, IList<dynamic>>()
        {
            ["Direct"] = new List<dynamic>() { 
                new { ExchangeName = "Direct-Exchange", QueueName = "Direct-QueueName-1" }, 
                new { ExchangeName = "Direct-Exchange", QueueName = "Direct-QueueName-2" } 
            },
            ["Fanout"] = new List<dynamic>() { 
                new { ExchangeName = "Fanout-Exchange", QueueName = "Fanout-QueueName-1" }, 
                new { ExchangeName = "Fanout-Exchange" , QueueName = "Fanout-QueueName-2" }, 
                new { ExchangeName = "Fanout-Exchange" , QueueName = "Fanout-QueueName-3" }, 
                new { ExchangeName = "Fanout-Exchange" , QueueName = "Fanout-QueueName-4" }, 
            },
            ["Topic"] = new List<dynamic>() { 
                new { ExchangeName = "Topic-Exchange-1", QueueName = "Topic-QueueName-1" }, 
                new { ExchangeName = "Topic-Exchange-1", QueueName = "Topic-All-QueueName-1" }, 
                new { ExchangeName = "Topic-Exchange-2", QueueName = "Topic-QueueName-2" } ,
                new { ExchangeName = "Topic-Exchange-2", QueueName = "Topic-All-QueueName-2" } 
            },
            ["Headers"] = new List<dynamic>() { 
                new { ExchangeName = "Headers-Exchange", QueueName = "Headers-QueueName-1" }, 
                new { ExchangeName = "Headers-Exchange", QueueName = "Headers-QueueName-2" } 
            },
        };

        public static Dictionary<string, IList<dynamic>> RoutingKey = new Dictionary<string, IList<dynamic>>()
        {
            ["Direct"] = new List<dynamic>() { 
                new { RoutingKey = "Direct-RoutingKey-1", QueueName = "Direct-QueueName-1" }, 
                new { RoutingKey = "Direct-RoutingKey-2", QueueName = "Direct-QueueName-2" } 
            },
            ["Fanout"] = new List<dynamic>() {},
            ["Topic"] = new List<dynamic>() { 
                new { RoutingKey = "Topic.*.Exchange-1", QueueName = "Topic-QueueName-1" }, 
                new { RoutingKey = "#.Exchange-1", QueueName = "Topic-All-QueueName-1" }, 
                new { RoutingKey = "Topic.*.Exchange-2", QueueName = "Topic-QueueName-2" },
                new { RoutingKey = "#.Exchange-2", QueueName = "Topic-All-QueueName-2" }
            },
            ["Headers"] = new List<dynamic>() { 
                new { RoutingKey = "Headers-Exchange-1", QueueName = "Headers-QueueName-1" }, 
                new { RoutingKey = "Headers-Exchange-2", QueueName = "Headers-QueueName-2" } 
            },
        };

        public ConnectionRabbitMQHepperExtension(ILogger<ConnectionRabbitMQHepperExtension> logger, ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
        public IModel Connect()
        {
            this._connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return this._channel;
            }

            _channel = _connection.CreateModel();


            QueueName.Keys.ToList().ForEach(key => {

                var exchangeName = ExchangeName[key].ToList();
                var queueName = QueueName[key].ToList();
                var routingKey = RoutingKey[key].ToList();

                queueName.ForEach(queue =>
                {
                    string queueNameKey = queue.GetType().GetProperty("QueueName").GetValue(queue, null);

                    exchangeName.ForEach(exchange => {
                        string exchangeName = exchange.GetType().GetProperty("ExchangeName").GetValue(exchange, null);
                        string queueNameCheck = exchange.GetType().GetProperty("QueueName").GetValue(exchange, null);
                        if(queueNameCheck == queueNameKey) {
                            string type = String.Empty;
                            switch (key)
                            {
                                case "Direct":
                                    type = ExchangeType.Direct;
                                    break;
                                case "Fanout":
                                    type = ExchangeType.Fanout;
                                    break;
                                case "Topic":
                                    type = ExchangeType.Topic;
                                    break;
                                case "Headers":
                                    type = ExchangeType.Headers;
                                    break;
                                default:
                                    type = ExchangeType.Topic;
                                    break;
                            }

                            if (key == "Fanout")
                            {
                                _channel.ExchangeDeclare(exchangeName, type: type, true, false);

                                _channel.QueueDeclare(queueNameKey, true, false, false, null);

                                _channel.QueueBind(exchange: exchangeName, queue: queueNameKey, routingKey: String.Empty);

                                _logger.LogInformation($"Connected RabbitMQ successfuly...Quêue : {queueNameKey}, Exchange :{exchangeName}");
                            }

                            routingKey.ForEach(routing => {

                                string routingName = routing.GetType().GetProperty("RoutingKey").GetValue(routing, null);
                                string queueNameRoutigCheck = routing.GetType().GetProperty("QueueName").GetValue(routing, null);

                                if (queueNameRoutigCheck == queueNameKey)
                                {
                                    _channel.ExchangeDeclare(exchangeName, type: type, true, false);

                                    _channel.QueueDeclare(queueNameKey, true, false, false, null);

                                    _channel.QueueBind(exchange: exchangeName, queue: queueNameKey, routingKey: routingName);

                                    _logger.LogInformation($"Connected RabbitMQ successfuly...Quêue : {queueNameKey}, Exchange :{exchangeName} , RoutingKey : {routingName}");
                                }

                            });
                        }
                        
                    });

                });
                
            });

            return _channel;
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
