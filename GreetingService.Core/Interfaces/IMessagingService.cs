using GreetingService.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreetingService.Core.Interfaces
{
    public interface IMessagingService
    {
        Task SendAsync<T>(T message, MessagingServiceSubject subject);
    }
}
