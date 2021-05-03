namespace src.Models.RequestModel
{
    public class UserAddClaimRequest
    {
        public string UserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
