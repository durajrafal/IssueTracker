using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.Account
{
    public class ResetPasswordViewModel
    {
        public string Email { get; init; }
        public string Token { get; init; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum Password length is 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is Required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum Password length is 6 characters")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
