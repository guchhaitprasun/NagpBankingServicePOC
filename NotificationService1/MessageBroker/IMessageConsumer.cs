using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService1.MessageBroker
{
    public interface IMessageConsumer
    {
        void ListenForNewNotifications();
    }
}
