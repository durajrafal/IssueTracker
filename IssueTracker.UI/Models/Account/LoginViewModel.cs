using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum Password length is 6 characters")]
        public string Password { get; set; }

        public bool IsRememberChecked { get; set; }

        public HtmlString? ResultMessage { get; set; }
    }
}