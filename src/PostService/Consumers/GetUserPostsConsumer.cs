using System.Threading.Tasks;
using BuildingBlocks.Models;
using MassTransit;

namespace PostService.Consumers
{
    public class GetUserPostsConsumer : IConsumer<GetUserPostsRequest>
    {
        private readonly IPostService _postService;

        public GetUserPostsConsumer(IPostService postService)
        {
            _postService = postService;
        }

        public async Task Consume(ConsumeContext<GetUserPostsRequest> context)
        {
            await context.RespondAsync<GetUserPostsResponse>(new GetUserPostsResponse()
            {
                Posts = await _postService.GetUserPosts(context.Message.User.Id)
            });
        }
    }
}