namespace PostService.Consumers
{
    // public class CheckUserPostsConsumer : IConsumer<CheckUserPosts>
    // {
    //     private readonly IPostService _postService;
    //
    //     public CheckUserPostsConsumer(IPostService postService)
    //     {
    //         _postService = postService;
    //     }
    //
    //     public async Task Consume(ConsumeContext<CheckUserPosts> context)
    //     {
    //         Console.WriteLine($"getting posts for {context.Message.Id}");
    //         if (context.Message.Id == null)
    //         {
    //             Console.WriteLine("Not Found");
    //             throw new InvalidOperationException("not found");
    //         }
    //
    //         // var posts = _postService.GetUsersPost(context.Message.Id);
    //         // var arrPosts = posts.ToArray();
    //         // string combinedString = string.Join(",", posts.ToArray());
    //         // CheckUserPostsResult result = new CheckUserPostsResult {stringList = combinedString};
    //         // Console.WriteLine(arrPosts);
    //
    //         // await context.RespondAsync<CheckUserPostsResult>(new CheckUserPostsResult {Posts = arrPosts});
    //     }
    // }
}