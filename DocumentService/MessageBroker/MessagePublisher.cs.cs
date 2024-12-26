using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.MessageBroker
{
    public class MessagePublisher<T> : IMessagePublisher<T>
    {
        private readonly BrokerConfiguration brokerConfiguration;
        private IConnection connection;
        private IModel channell;


        public MessagePublisher()
        {
            brokerConfiguration = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DocumentsQueue = "document_notification_queue",
                Exchange = "nagp_fanout_exchange"
            };
        }

        public async Task PublishMessageAsync(T message)
        {
            using (connection = SetupRabbitMqBrokerConnection())
            {
                using (channell = CreateRabbitMqBrokerChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(brokerConfiguration.Exchange, ExchangeType.Fanout, false, false, null);

                    //Queue Configuration
                    channell.QueueDeclare(queue: brokerConfiguration.DocumentsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var body = CreateBody(message);

                    await Task.Run(() => channell.BasicPublish(brokerConfiguration.Exchange, string.Empty, false, null, body));
                }
            }
        }

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
    }
}
