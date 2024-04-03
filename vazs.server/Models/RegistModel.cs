using System.ComponentModel.DataAnnotations;

namespace vazs.server.Models
{
    public class RegistModel
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Username { get; set; }
    }
}
