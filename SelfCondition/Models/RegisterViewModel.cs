using System.ComponentModel.DataAnnotations;

namespace SelfCondition.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(30, ErrorMessage = "Username can be max 30 characters.")]
        public string Username { get; set; }

        //[DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password can be min 6 characters.")]
        [MaxLength(16, ErrorMessage = "Password can be max 16 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-Password is required.")]
        [MinLength(6, ErrorMessage = "Re-Password can be min 6 characters.")]
        [MaxLength(16, ErrorMessage = "Re-Password can be max 16 characters.")]
        [Compare(nameof(Password))] //nameof: property (Password) değiştiğinde hata verip düzenlemeye yardım eder.
        public string RePassword { get; set; }
    }
}
