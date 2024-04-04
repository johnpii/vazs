﻿using System.ComponentModel.DataAnnotations;

namespace vazs.server.Models
{
    public class TSModel
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
        public decimal Budget { get; set; }
        public IFormFile? Document { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string ClientID { get; set; }

        [Required(ErrorMessage = "Это поле обязательно")]
        public string DepartmentName { get; set; }
    }
}
