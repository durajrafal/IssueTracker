using IssueTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    public class EmailController : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Confirm(string token, string email, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("Error");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "Confirm" : "Error");
        }
    }
}
