using System;
using System.Threading.Tasks;
using AnalyticsService.Models;
using AnalyticsService.Services;
using MassTransit;

namespace AnalyticsService.Consumers
{
    public class PostConsumer : IConsumer<PostEvent>
    {
        private readonly IPostEventService _postEventService;

        public PostConsumer(IPostEventService postEventService)
        {
            _postEventService = postEventService;
        }
        
        public async Task Consume(ConsumeContext<PostEvent> context)
        {
            Console.WriteLine("Got user event");
            await _postEventService.Create(context.Message);
        }
    }
}