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
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NotificationService2.MessageBroker
{
    public class MessageConsumer : IMessageConsumer, IDisposable
    {
        private readonly BrokerConfiguration _fanoutconfig;
        private readonly BrokerConfiguration _topicConfig;

        private readonly IConnection brokerConnection;
        private readonly IModel brokerChannell;
        private bool _disposed = false;


        public MessageConsumer(BrokerConfiguration topicConfig, BrokerConfiguration fanoutConfig)
        {
            _fanoutconfig = fanoutConfig;
            _topicConfig = topicConfig;

            brokerConnection = SetupRabbitMqConnection(topicConfig);
            brokerChannell = SetupRabbitMqChannell(brokerConnection);
        }

        public void ListneToNewAccountCreationEvent()
        {
            brokerChannell.QueueBind(_topicConfig.AccountsQueue, _topicConfig.Exchange, _topicConfig.AccountsQueue, null);

            var consumer = new EventingBasicConsumer(brokerChannell);

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
            brokerChannell.BasicConsume(_topicConfig.AccountsQueue, true, consumer);

        }

        public void ListenToNewNotification()
        {
            brokerChannell.QueueBind(_fanoutconfig.AccountsQueue, _fanoutconfig.Exchange, string.Empty, null);

            var consumer = new EventingBasicConsumer(brokerChannell);

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
            brokerChannell.BasicConsume(_fanoutconfig.AccountsQueue, true, consumer);

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

        #region Private functions
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
            var channell =  connection.CreateModel();
            channell.QueueDeclare(queue: _fanoutconfig.AccountsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            return channell;
        }
        #endregion
    }
}
