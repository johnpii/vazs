using System.ComponentModel.DataAnnotations;

namespace vazs.server.ViewModels
{
    public class TSViewModelForUpdate
    {
        [Required(ErrorMessage = "Это поле обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public DateTime Deadline { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public int Budget { get; set; }
        public IFormFile? Document { get; set; }
    }
}
