using System.ComponentModel.DataAnnotations;

namespace vazs.server.ViewModels
{
    public class DepartmentModelForCreate
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public IFormFile Image { get; set; }
    }
}
