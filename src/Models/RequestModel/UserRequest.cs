using System.ComponentModel.DataAnnotations;

namespace src.Models.RequestModel
{
    public class UserRequest
    {
        [Required(ErrorMessage ="Email tidak boleh kosong")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password tidak boleh kosong")]
        public string Password { get; set; }
    }
}
