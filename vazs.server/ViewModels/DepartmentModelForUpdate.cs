using System.ComponentModel.DataAnnotations;

namespace vazs.server.ViewModels
{
    public class DepartmentModelForUpdate
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
