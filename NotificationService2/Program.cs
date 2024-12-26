using NotificationService2.MessageBroker;
using SharedProject.DTOs;

namespace NotificationService2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**************** NOTIFICATION SERVICE 2 *****************\n\n");
            Thread.Sleep(1500);
            Console.WriteLine("Service Booting Up....");
            Thread.Sleep(3000);
            Console.WriteLine("Service Up and Listning for new messeges from queue");


            BrokerConfiguration topicConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AccountsQueue = "account_queue",
                Exchange = "nagp_topic_exchange"
            };

            BrokerConfiguration fanoutConfig = new BrokerConfiguration
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                AccountsQueue = "document_notification_queue_2",
                Exchange = "nagp_fanout_exchange"
            };

            IMessageConsumer consumer = new MessageConsumer(topicConfig, fanoutConfig);


            while (true)
            {
                try
                {
                    consumer.ListneToNewAccountCreationEvent();
                    consumer.ListenToNewNotification();
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
