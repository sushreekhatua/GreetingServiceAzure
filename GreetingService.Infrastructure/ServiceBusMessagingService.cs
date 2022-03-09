using Azure.Messaging.ServiceBus;
using GreetingService.Core.Enums;
using GreetingService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreetingService.Infrastructure
{
    public class ServiceBusMessagingService : IMessagingService
    {

        private readonly ServiceBusSender _servicebussender;
        public ServiceBusMessagingService(ServiceBusSender servicebussender)
        {
            _servicebussender = servicebussender;
        }
        public async Task SendAsync<T>(T message, MessagingServiceSubject subject)
        {
            //var mymessage = message;
            var servicebusmessage = new ServiceBusMessage(JsonSerializer.Serialize(message))
            {
                Subject = subject.ToString()
            };
            await _servicebussender.SendMessageAsync(servicebusmessage);
        }

    }
}
