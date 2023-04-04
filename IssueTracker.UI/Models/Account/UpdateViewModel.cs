using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.Account
{
    public class UpdateViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Email { get; set; }


        [Required(ErrorMessage = "First Name is Required")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }


        public bool PasswordChangeRequested { get; set; }

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
