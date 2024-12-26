using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService1.MessageBroker
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly BrokerConfiguration _accountCreationBrokerConfig;

        public MessageConsumer()
        {
            _accountCreationBrokerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DocumentsQueue = "document_notification_queue",
                Exchange = "nagp_fanout_exchange"
            };
        }

        public void ListenForNewNotifications()
        {
            using (IConnection connection = SetupRabbitMqConnection(_accountCreationBrokerConfig))
            {
                using (IModel channell = SetupRabbitMqChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(_accountCreationBrokerConfig.Exchange, ExchangeType.Fanout);

                    //Queue Configuration
                    channell.QueueDeclare(queue: _accountCreationBrokerConfig.DocumentsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    channell.QueueBind(_accountCreationBrokerConfig.DocumentsQueue, _accountCreationBrokerConfig.Exchange, string.Empty, null);

                    var consumer = new EventingBasicConsumer(channell);

                    consumer.Received += (sender, args) =>
                    {
                        var body = args.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[x] Received New Notification: {message}");

                        Console.WriteLine(":::: Waiting for Next Message ::::");

                    };

                    // subscribe to the queue
                    channell.BasicConsume(_accountCreationBrokerConfig.DocumentsQueue, true, consumer);
                }
            }
        }

        private IConnection SetupRabbitMqConnection(BrokerConfiguration brokerConfiguration)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = brokerConfiguration.HostName,
                UserName = brokerConfiguration.UserName,
                Password = brokerConfiguration.Password,
            };

            return connectionFactory.CreateConnection();
        }

        private IModel SetupRabbitMqChannell(IConnection connection)
        {
            return connection.CreateModel();
        }
    }
}
