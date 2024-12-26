using DocumentService.gRPC.Client;
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

namespace DocumentService.MessageBroker
{
    public class DocumentRequestMessageConsumer : IDocumentRequestMessageConsumer, IDisposable
    {
        private readonly BrokerConfiguration _consumerConfig;
        private readonly BrokerConfiguration _producerConfig;

        private readonly IConnection brokerConnection;
        private readonly IModel brokerChannell;
        private bool _disposed = false;

        public DocumentRequestMessageConsumer(BrokerConfiguration config, BrokerConfiguration producerConfig)
        {
            _consumerConfig = config;
            _producerConfig = producerConfig;

            brokerConnection = SetupRabbitMqConnection(config);
            brokerChannell = SetupRabbitMqChannell(brokerConnection);
        }

        public void ListneForNewDocumentCreationRequest()
        {
            brokerChannell.QueueBind(_consumerConfig.DocumentsQueue, _consumerConfig.Exchange, _consumerConfig.DocumentsQueue, null);

            var consumer = new EventingBasicConsumer(brokerChannell);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received New Notification:");

                var resp = JsonConvert.DeserializeObject<AccountStatementRequestDTO>(message);

                if (resp != null)
                    ProcessPDFGeneration(resp);

                else
                    Console.WriteLine("Invalid Message Received");

                Console.WriteLine(":::: Waiting for Next Message ::::");

            };

            // subscribe to the queue
            brokerChannell.BasicConsume(_consumerConfig.DocumentsQueue, true, consumer);
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

        #region Private Implementation

        private void ProcessPDFGeneration(AccountStatementRequestDTO pdfGenerationRequest)
        {
            AccountRpcClientService accountRpcClientService = new AccountRpcClientService();

            var payload = JsonConvert.SerializeObject(pdfGenerationRequest);

            var status = accountRpcClientService.FetchAndGenerateAccountStatementPDF(payload);

            if (status)
            {
                IMessagePublisher<string> messagePublisher = new MessagePublisher<string>(_producerConfig);
                messagePublisher.PublishMessageAsync($"New PDF Statement Generated for Account Number {pdfGenerationRequest.AccountNumber}");
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

        #endregion

        // Destructor (finalizer) for unmanaged cleanup
        ~DocumentRequestMessageConsumer()
        {
            Dispose(false);
        }
    }
}
