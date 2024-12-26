using DocumentService.MessageBroker;

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

            while (true)
            {
                try
                {
                    IDocumentRequestMessageConsumer consumer = new DocumentRequestMessageConsumer();
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
