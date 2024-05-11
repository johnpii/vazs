using System.ComponentModel.DataAnnotations;

namespace vazs.server.ViewModels
{
    public class TSViewModelForCreate
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        [Range(1, float.MaxValue, ErrorMessage = "Цена должна быть выше 0")]
        public int Budget { get; set; }
        public IFormFile? Document { get; set; }
        public string? DocumentExt { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string ClientEmail { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string DepartmentName { get; set; }
    }
}