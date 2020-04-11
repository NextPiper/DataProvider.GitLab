using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DataProvider.GitLab.Managers.RabbitMQManager
{
    public interface IRabbitMQManager
    {
        Task InitClient();
        Task PublishMessageAsync<TMessage>(TMessage message);
    }
    
    public class RabbitMQManager : IRabbitMQManager
    {
        private RabbitMQConfig config;
        private const string GITLAB_COMMIT_EXCHANGE = "gitlab-commitexchange";
        
        public RabbitMQManager()
        {
        }

        public async Task InitClient()
        {
            // Fetch rabbitMQ client details from the controlplane
            var httpClient = new HttpClient();
            var url = $"http://nextpipe-service.default.svc.cluster.local:5555/core/config/rabbitmq?loadBalancer=false";

            var result = await httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                throw new SystemException("Couldn't resolve rabbitMQ configurations!");
            }
            
            var content = await result.Content.ReadAsStringAsync();
            config = JsonConvert.DeserializeObject<RabbitMQConfig>(content);
        }

        public async Task PublishMessageAsync<TMessage>(TMessage message)
        {
            // if ready, then publish the msg to rabbitMQ...
            try
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = config.Hostname,
                    UserName = config.Username,
                    Password = config.Password,
                    Port = config.Port
                };

                using (var rabbitConnection = connectionFactory.CreateConnection())
                {
                    using (var channel = rabbitConnection.CreateModel())
                    {
                        channel.ExchangeDeclare(GITLAB_COMMIT_EXCHANGE, ExchangeType.Fanout);

                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                        channel.BasicPublish(
                            exchange: GITLAB_COMMIT_EXCHANGE,
                            routingKey: "",
                            basicProperties: null,
                            body);
                        
                        Console.WriteLine("DataProvider.GitLab published commit to rabbitmq");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}