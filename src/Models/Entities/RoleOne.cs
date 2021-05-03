using Microsoft.AspNetCore.Identity;

namespace src.Models.Entities
{
    public class RoleOne : IdentityRole
    {
        public RoleOne()
        {

        }
        public RoleOne(string roleName) : base(roleName)
        {

        }
        public string Description { get; set; }
    }
}
