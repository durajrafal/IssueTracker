using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.Account
{
    public class ForgotPasswordViewModel : CustomErrorMessageModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}