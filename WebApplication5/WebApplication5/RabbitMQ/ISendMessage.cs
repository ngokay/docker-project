namespace WebApplication5.RabbitMQ
{
    public interface ISendMessage
    {
        void Send(dynamic message);
        void CreateQueue(Dictionary<string, IList<dynamic>> msgs);
    }
}
