using System.Threading.Tasks;
using BuildingBlocks.Models;
using MassTransit;

namespace PostService.Consumers
{
    public class DestroyUserPostsConsumer : IConsumer<DestroyUserPostsRequest>
    {
        private readonly IPostService _postService;

        public DestroyUserPostsConsumer(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Consume(ConsumeContext<DestroyUserPostsRequest> context)
        {
            await context.RespondAsync<DestroyUserPostsResponse>(new DestroyUserPostsResponse()
            {
                Destroyed = await _postService.DestroyUserPosts(context.Message.User)
            });
        }
    }
}