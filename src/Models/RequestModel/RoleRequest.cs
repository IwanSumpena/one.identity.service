using System.ComponentModel.DataAnnotations;

namespace src.Models.RequestModel
{
    public class RoleRequest
    {
        [Required(ErrorMessage = "RoleName tidak boleh kosong")]
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
