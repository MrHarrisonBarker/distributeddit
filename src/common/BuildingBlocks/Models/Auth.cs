namespace BuildingBlocks.Models
{
    public class AuthToken
    {
        public string Token { get; set; }
        public int Expires { get; set; }
    }

    public class Authenticate
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AuthContract
    {
        public string Email { get; set; }
    }
}