using System;
using System.Threading.Tasks;
using AnalyticsService.Models;
using AnalyticsService.Services;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class UserConsumer : IConsumer<UserEvent>
    {
        private readonly IUserEventService _userEventService;

        public UserConsumer(IUserEventService postEventService)
        {
            _userEventService = postEventService;
        }
        
        public async Task Consume(ConsumeContext<UserEvent> context)
        {
            Console.WriteLine("Got post event");
            await _userEventService.Create(context.Message);
        }
    }
}