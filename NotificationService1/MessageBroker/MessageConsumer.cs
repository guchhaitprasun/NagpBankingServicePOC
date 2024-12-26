using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NotificationService1.MessageBroker
{
    public class MessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly BrokerConfiguration _consumerConfig;

        private readonly IConnection brokerConnection;
        private readonly IModel brokerChannell;
        private bool _disposed = false;

        public MessageConsumer(BrokerConfiguration config)
        {
            _consumerConfig = config;

            brokerConnection = SetupRabbitMqConnection(config);
            brokerChannell = SetupRabbitMqChannell(brokerConnection);
        }

        public void ListenForNewNotifications()
        {
            brokerChannell.QueueBind(_consumerConfig.AccountsQueue, _consumerConfig.Exchange, string.Empty, null);

            var consumer = new EventingBasicConsumer(brokerChannell);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received New Notification: {message}");

                Console.WriteLine(":::: Waiting for Next Message ::::");

            };

            // subscribe to the queue
            brokerChannell.BasicConsume(_consumerConfig.AccountsQueue, true, consumer);

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Suppress finalization to avoid redundant cleanup
        }

        // Protected implementation of Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    brokerChannell?.Dispose();
                    brokerConnection?.Dispose();
                }

                _disposed = true;
            }
        }

        ~MessageConsumer()
        {
            Dispose(false);
        }

        #region Private

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
            var channell = connection.CreateModel();
            channell.QueueDeclare(queue: _consumerConfig.AccountsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            return channell;
        }

        #endregion
    }
}
