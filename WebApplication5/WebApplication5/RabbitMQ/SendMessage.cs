using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace WebApplication5.RabbitMQ
{
    public class SendMessage : ISendMessage
    {
        private readonly ILogger<SendMessage> logger;
        private readonly ConnectionRabbitMQHepper _connectionRabbitMQHepper;
        private readonly ConnectionRabbitMQHepperExtension _connectionRabbitMQHepperExtension;
        public SendMessage(ILogger<SendMessage> logger, ConnectionRabbitMQHepper _connectionRabbitMQHepper, 
            ConnectionRabbitMQHepperExtension _connectionRabbitMQHepperExtension)
        {
            this.logger = logger;
            this._connectionRabbitMQHepper = _connectionRabbitMQHepper;
            this._connectionRabbitMQHepperExtension = _connectionRabbitMQHepperExtension;
        }

        public void CreateQueue(Dictionary<string, IList<dynamic>> msgs)
        {
            try
            {
                using (var channel = _connectionRabbitMQHepperExtension.Connect())
                {

                    ConnectionRabbitMQHepperExtension.QueueName.Keys.ToList().ForEach(key => {

                        var messages = msgs[key].ToList();

                        messages.ForEach(msgs =>
                        {
                            string routingKey = msgs.GetType().GetProperty("RoutingKey").GetValue(msgs, null);
                            string exchangeName = msgs.GetType().GetProperty("ExchangeName").GetValue(msgs, null);
                            string msg = JsonConvert.SerializeObject(msgs.GetType().GetProperty("Msg").GetValue(msgs, null));
                            var body = Encoding.UTF8.GetBytes(msg);

                            // đánh dấu tin nhắn là liên tục
                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;

                            channel.BasicPublish(
                                exchange: exchangeName,
                                routingKey: routingKey,
                                basicProperties: properties,
                                body: body
                            );

                        });

                    });

                }

                logger.LogInformation("Send message to rabbit succes");
            }
            catch (Exception ex)
            {
                logger.LogError("Send message to rabbit error : " + ex.Message);
            }
        }

        public void Send(dynamic message)
        {
            try
            {
                using (var channel = _connectionRabbitMQHepper.Connect())
                {
                    string bodyMsg = JsonConvert.SerializeObject(message);

                    var body = Encoding.UTF8.GetBytes(bodyMsg);
                    channel.BasicPublish(
                        exchange: ConnectionRabbitMQHepper.ExchangeName,
                        routingKey: ConnectionRabbitMQHepper.RoutingWeatherForecast,
                        basicProperties: null,
                        body: body
                    );

                }

                logger.LogInformation("Send message to rabbit succes");
            }
            catch (Exception ex)
            {
                logger.LogError("Send message to rabbit error : " + ex.Message);
            }
            
        }
    }
}
