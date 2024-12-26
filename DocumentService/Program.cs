using DocumentService.MessageBroker;
using SharedProject.DTOs;

namespace DocumentService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**************** DOCUMENT SERVICE *****************\n\n");
            Thread.Sleep(1500);
            Console.WriteLine("Service Booting Up....");
            Thread.Sleep(3000);
            Console.WriteLine("Service Up and Listning for new messeges from queue");


            BrokerConfiguration consumerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DocumentsQueue = "document_queue",
                Exchange = "nagp_topic_exchange"
            };

            BrokerConfiguration publisherConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                DocumentsQueue = "document_notification_queue",
                Exchange = "nagp_fanout_exchange"
            };

            IDocumentRequestMessageConsumer consumer = new DocumentRequestMessageConsumer(consumerConfig, publisherConfig);

            while (true)
            {
                try
                {
                    consumer.ListneForNewDocumentCreationRequest();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occured in Documents Service");
                    Console.WriteLine(ex.Message);
                }

                // Sleep to prevent high CPU usage
                Thread.Sleep(1000);
            }
        }
    }
}
