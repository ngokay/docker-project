using StackExchange.Redis;

namespace WebApplication5.Redis
{
    public class ConnectionHelper
    {
        private static string GetAppSteting()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build().GetSection("RedisUrl").Value;
#pragma warning restore CS8603 // Possible null reference return.
        }
        static ConnectionHelper()
        {
            ConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
                return ConnectionMultiplexer.Connect(GetAppSteting());
            });
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
