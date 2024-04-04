using System.ComponentModel.DataAnnotations;

namespace vazs.server.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля 6 символов")]
        public string Password { get; set; }
    }
}
