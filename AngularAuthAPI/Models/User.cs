using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string ?FirstName { get; set; }

        public string ?LastName { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        private string? _token;

        private string? _role;

        public string? Token
        {
            get { return string.IsNullOrEmpty(_token) ? "1" : _token; }
            set { _token = value; }
        }
        public string? Role
        {
            get { return string.IsNullOrEmpty(_role) ? "1" : _role; }
            set { _role = value; }
        }
        public string ?Email { get; set; }
    }

}
