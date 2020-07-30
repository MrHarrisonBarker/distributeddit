using System.Threading.Tasks;
using BuildingBlocks.Models;
using MassTransit;

namespace UserService.Consumers
{
    public class DestroyPostConsumer : IConsumer<DestroyPostRequest>
    {
        private readonly IUserService _userService;

        public DestroyPostConsumer(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task Consume(ConsumeContext<DestroyPostRequest> context)
        {
            await context.RespondAsync<DestroyPostResponse>(new DestroyPostResponse()
            {
                Destroyed = await _userService.DestroyPost(context.Message.Id)
            });
        }
    }
}