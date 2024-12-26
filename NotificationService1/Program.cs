using NotificationService1.MessageBroker;
using SharedProject.DTOs;

namespace NotificationService1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**************** NOTIFICATION SERVICE 1 *****************\n\n");
            Thread.Sleep(1500);
            Console.WriteLine("Service Booting Up....");
            Thread.Sleep(3000);
            Console.WriteLine("Service Up and Listning for new messeges from queue");

            BrokerConfiguration consumerConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AccountsQueue = "document_notification_queue_1",
                Exchange = "nagp_fanout_exchange"
            };

            IMessageConsumer consumer = new MessageConsumer(consumerConfig);

            while (true)
            {
                try
                {
                    consumer.ListenForNewNotifications();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occured in Notification Service");
                    Console.WriteLine(ex.Message);
                }

                // Sleep to prevent high CPU usage
                Thread.Sleep(1000);
            }
        }
    }
}
