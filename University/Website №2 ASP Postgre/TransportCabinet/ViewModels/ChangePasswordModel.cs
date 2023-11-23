using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.ViewModels
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Пароли не совпадают")]
        public string confirmPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
    }
}