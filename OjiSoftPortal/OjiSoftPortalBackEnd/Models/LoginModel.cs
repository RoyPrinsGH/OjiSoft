namespace OjiSoftPortal.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginModel
    {
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
        public required string ReturnURL { get; set; }
    }
}
