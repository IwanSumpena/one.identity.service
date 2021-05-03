namespace src.Models.RequestModel
{
    public class UserSetEnableLockoutRequest
    {
        public string UserId { get; set; }
        public string StatusEnableLockout { get; set; }
    }
}
