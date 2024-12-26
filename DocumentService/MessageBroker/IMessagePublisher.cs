using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.MessageBroker
{
    public interface IMessagePublisher<T>
    {
        Task PublishMessageAsync(T message);
    }
}
