using System;
using System.Threading.Tasks;
using BuildingBlocks.Models;
using MassTransit;

namespace UserService.Consumers
{
    public class AddPostConsumer : IConsumer<Post>
    {
        private readonly IUserService _userService;

        public AddPostConsumer(IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<Post> context)
        {
            await context.RespondAsync<AddPostResponse>(new {Added = await _userService.AddPost(context.Message)});
        }
    }
}