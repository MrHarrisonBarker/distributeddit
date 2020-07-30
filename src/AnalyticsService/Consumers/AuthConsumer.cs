using System;
using System.Threading.Tasks;
using AnalyticsService.Models;
using AnalyticsService.Services;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class AuthConsumer : IConsumer<AuthEvent>
    {
        private readonly IAuthEventService _authEventService;

        public AuthConsumer(IAuthEventService postEventService)
        {
            _authEventService = postEventService;
        }
        
        public async Task Consume(ConsumeContext<AuthEvent> context)
        {
            Console.WriteLine("Got auth event");
            await _authEventService.Create(context.Message);
        }
    }
}