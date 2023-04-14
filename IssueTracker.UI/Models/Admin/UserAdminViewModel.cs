using System.ComponentModel;

namespace IssueTracker.UI.Models.Admin
{
    public class UserAdminViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Role")]
        public string RoleClaim { get; set; }
    }
}
