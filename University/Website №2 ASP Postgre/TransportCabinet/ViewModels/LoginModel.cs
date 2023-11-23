using System.ComponentModel.DataAnnotations;

namespace TransportCabinet.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
