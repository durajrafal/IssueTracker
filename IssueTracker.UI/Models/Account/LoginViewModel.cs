using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.Account
{
    public class LoginViewModel
    {

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRememberChecked { get; set; }
    }
}
