using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class EmailController : Controller
    {
        #region Confirm
        [HttpGet]
        public async Task<IActionResult> Confirm(string token, string email, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToRegisterWithConfirmationError(email);
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var html = "<h4>Congratulations!</h4>Your email is confirmed. Now you can log in with your account.";
                TempData["EmailActionSuccess"] = html;
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToRegisterWithConfirmationError(email);
            }
        }

        private IActionResult RedirectToRegisterWithConfirmationError(string email)
        {
            TempData["EmailActionError"] = $"Impossible to confirm your email '{email}'.<strong> Register again.</strong>";
            return RedirectToAction("Register", "Account");
        }
        #endregion
            return RedirectToAction("Register", "Account");
        }
    }
}
