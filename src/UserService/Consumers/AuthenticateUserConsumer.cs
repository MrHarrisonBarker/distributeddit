using System;
using System.Threading.Tasks;
using BuildingBlocks.Models;
using MassTransit;

namespace UserService.Consumers
{
    public class AuthenticateUserConsumer: IConsumer<AuthContract>
    {
        private readonly IUserService _userService;

        public AuthenticateUserConsumer(IUserService userService)
        {
            _userService = userService;
        }
        
        public async Task Consume(ConsumeContext<AuthContract> context)
        {
            Console.WriteLine($"Getting user -> {context.Message.Email} from db");
            
            var user = await _userService.GetByEmail(context.Message.Email);

            if (user == null)
            {
                Console.WriteLine("Not Found");
                throw new InvalidOperationException("not found");
            }
            
            Console.WriteLine($"got user from the db {user.DisplayName}");

            await context.RespondAsync<BuildingBlocks.Models.User>(user);
        }
    }
}