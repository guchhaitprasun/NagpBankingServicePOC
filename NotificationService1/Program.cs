using NotificationService1.MessageBroker;

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

            while (true)
            {
                try
                {
                    IMessageConsumer consumer = new MessageConsumer();
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
