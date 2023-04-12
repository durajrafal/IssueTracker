using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IssueTracker.UI.Models.Admin
{
    public class UpdateUserRoleClaimViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string UserId { get; init; }
        public SelectList? Roles { get; init; }
        [Required]
        public string SelectedRole { get; set; }

        public UpdateUserRoleClaimViewModel()
        {
            
        }

        public UpdateUserRoleClaimViewModel(IList<Claim> claims, string userId)
        {
            SelectedRole = claims.FirstOrDefault(x => x.Type.EndsWith("role"))?.Value;
            Roles = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{Text = "SelectRole", Value = ""},
                    new SelectListItem{Selected=true, Text = "Developer", Value = "Developer"},
                    new SelectListItem{Text = "Manager", Value = "Manager"},
                    new SelectListItem{Text = "Admin", Value = "Admin"},
                }, "Value", "Text");
            UserId = userId;
        }

    }
}
