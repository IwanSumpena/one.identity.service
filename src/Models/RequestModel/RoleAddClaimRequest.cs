namespace src.Models.RequestModel
{
    public class RoleAddClaimRequest
    {
        public string RoleName { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
