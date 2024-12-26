using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotificationService2.MessageBroker
{
    public class MessageConsumer : IMessageConsumer
    {
        private readonly BrokerConfiguration _accountCreationBrokerConfig;
        private readonly BrokerConfiguration _accountCreationNotificationBrokerConfig;


        public MessageConsumer()
        {
            _accountCreationBrokerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AccountsQueue = "account_queue",
                Exchange = "nagp_topic_exchange"
            };

            _accountCreationNotificationBrokerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AccountsQueue = "document_notification_queue",
                Exchange = "nagp_fanout_exchange"
            };
        }

        public void ListneToNewAccountCreationEvent()
        {
            using (IConnection connection = SetupRabbitMqConnection(_accountCreationBrokerConfig))
            {
                using (IModel channell = SetupRabbitMqChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(_accountCreationBrokerConfig.Exchange, ExchangeType.Topic);

                    //Queue Configuration
                    channell.QueueDeclare(queue: _accountCreationBrokerConfig.AccountsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    channell.QueueBind(_accountCreationBrokerConfig.AccountsQueue, _accountCreationBrokerConfig.Exchange, _accountCreationBrokerConfig.AccountsQueue, null);

                    var consumer = new EventingBasicConsumer(channell);

                    consumer.Received += (sender, args) =>
                    {
                        var body = args.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[x] Received New Notification From Topic Exchange:");

                        var resp = JsonConvert.DeserializeObject(message);

                        Console.WriteLine(resp);

                        Console.WriteLine(":::: Waiting for Next Message ::::");

                    };

                    // subscribe to the queue
                    channell.BasicConsume(_accountCreationBrokerConfig.AccountsQueue, true, consumer);
                }
            }
        }

        public void ListenToNewNotification()
        {
            using (IConnection connection = SetupRabbitMqConnection(_accountCreationNotificationBrokerConfig))
            {
                using (IModel channell = SetupRabbitMqChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(_accountCreationNotificationBrokerConfig.Exchange, ExchangeType.Fanout);

                    //Queue Configuration
                    channell.QueueDeclare(queue: _accountCreationNotificationBrokerConfig.AccountsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    channell.QueueBind(_accountCreationNotificationBrokerConfig.AccountsQueue, _accountCreationNotificationBrokerConfig.Exchange, string.Empty, null);

                    var consumer = new EventingBasicConsumer(channell);

                    consumer.Received += (sender, args) =>
                    {
                        var body = args.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[x] Received New Notification From Fanout Exchange:");

                        var resp = JsonConvert.DeserializeObject(message);

                        Console.WriteLine(resp);

                        Console.WriteLine(":::: Waiting for Next Message ::::");

                    };

                    // subscribe to the queue
                    channell.BasicConsume(_accountCreationBrokerConfig.AccountsQueue, true, consumer);
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
