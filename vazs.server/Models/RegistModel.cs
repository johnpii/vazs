using System.ComponentModel.DataAnnotations;

namespace vazs.server.Models
{
    public class RegistModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }
    }
}
