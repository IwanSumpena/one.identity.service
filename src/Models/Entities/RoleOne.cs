using Microsoft.AspNetCore.Identity;

namespace src.Models.Entities
{
    public class RoleOne : IdentityRole
    {
        public string Description { get; set; }
    }
}
