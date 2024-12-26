using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedProject.DTOs;
using System.Text;

namespace AccountService.MessageBroker
{
    public class QueuePublisher<T> : IQueuePublisher<T>
    {
        private readonly BrokerConfiguration brokerConfiguration;
        private IConnection connection;
        private IModel channell;

        public QueuePublisher(IConfiguration configuration)
        {
            brokerConfiguration = configuration.GetSection("RabbitMQ").Get<BrokerConfiguration>();
        }

        public async Task PublishMessageAsync(T message) 
        {
            using (connection = SetupRabbitMqBrokerConnection())
            {
                using (channell = CreateRabbitMqBrokerChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(brokerConfiguration.Exchange, ExchangeType.Topic, false, false, null);

                    //Queue Configuration
                    channell.QueueDeclare(queue: brokerConfiguration.AccountsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    
                    var body = CreateBody(message);

                    await Task.Run(() => channell.BasicPublish(brokerConfiguration.Exchange, brokerConfiguration.AccountsQueue, false, null, body));
                }
            }
        }

        public async Task PublishAccountStatementPdfRequestAsync(T message)
        {
            using (connection = SetupRabbitMqBrokerConnection())
            {
                using (channell = CreateRabbitMqBrokerChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(brokerConfiguration.Exchange, ExchangeType.Topic, false, false, null);

                    //Queue Configuration
                    channell.QueueDeclare(queue: brokerConfiguration.DocumentsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = CreateBody(message);

                    await Task.Run(() => channell.BasicPublish(brokerConfiguration.Exchange, brokerConfiguration.DocumentsQueue, false, null, body));
                }
            }
        }

        #region Private Region

        private IConnection SetupRabbitMqBrokerConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = brokerConfiguration.HostName,
                UserName = brokerConfiguration.UserName,
                Password = brokerConfiguration.Password,    
            };

            return connectionFactory.CreateConnection();
        }

        private IModel CreateRabbitMqBrokerChannell(IConnection connection)
        {
            return connection.CreateModel();
        }

        // Encode message body
        private byte[] CreateBody<T>(T command)
        {
            var message = JsonConvert.SerializeObject(command);
            return Encoding.UTF8.GetBytes(message);
        }

       
        #endregion
    }
}
