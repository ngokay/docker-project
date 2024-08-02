using RabbitMQ.Client;

namespace WebApplication5.RabbitMQ
{
    public interface IConnectionRabbitMQ
    {
        IConnection Init();
    }
}
