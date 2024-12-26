using DocumentService.gRPC.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.MessageBroker
{
    public class DocumentRequestMessageConsumer : IDocumentRequestMessageConsumer
    {
        private readonly BrokerConfiguration _accountCreationBrokerConfig;

        public DocumentRequestMessageConsumer()
        {
            _accountCreationBrokerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DocumentsQueue = "document_queue",
                Exchange = "nagp_topic_exchange"
            };
        }

        public void ListneForNewDocumentCreationRequest()
        {
            using (IConnection connection = SetupRabbitMqConnection(_accountCreationBrokerConfig))
            {
                using (IModel channell = SetupRabbitMqChannell(connection))
                {
                    // Exchange Configuration 
                    channell.ExchangeDeclare(_accountCreationBrokerConfig.Exchange, ExchangeType.Topic);

                    //Queue Configuration
                    channell.QueueDeclare(queue: _accountCreationBrokerConfig.DocumentsQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    channell.QueueBind(_accountCreationBrokerConfig.DocumentsQueue, _accountCreationBrokerConfig.Exchange, _accountCreationBrokerConfig.DocumentsQueue, null);

                    var consumer = new EventingBasicConsumer(channell);

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
                    channell.BasicConsume(_accountCreationBrokerConfig.DocumentsQueue, true, consumer);
                }
            }
        }

        private void ProcessPDFGeneration(AccountStatementRequestDTO pdfGenerationRequest)
        {
            AccountRpcClientService accountRpcClientService = new AccountRpcClientService();

            var payload = JsonConvert.SerializeObject(pdfGenerationRequest);

            accountRpcClientService.FetchAndGenerateAccountStatementPDF(payload);
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
