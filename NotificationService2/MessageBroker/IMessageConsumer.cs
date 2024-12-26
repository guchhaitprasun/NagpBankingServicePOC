using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService2.MessageBroker
{
    public interface IMessageConsumer
    {
        void ListneToNewAccountCreationEvent();
        void ListenToNewNotification();
    }
}
