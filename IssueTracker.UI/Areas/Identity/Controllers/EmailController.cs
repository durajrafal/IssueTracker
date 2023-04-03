using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        #region ResetPassword
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var vm = new ResetPasswordViewModel { Token = token, Email = email };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm, [FromServices] UserManager<ApplicationUser> userManager)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await userManager.FindByEmailAsync(vm.Email);
            if (user == null)
                return RedirectToRegisterWithPasswordResetError(vm.Email);

            var resetPasswordResult = await userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
            if (resetPasswordResult.Succeeded)
            {
                TempData["EmailActionSuccess"] = $"<h4>Congratulations!</h4>Your password is changed. Now you can log in with your account using new password.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(vm);
            }
        }

        private IActionResult RedirectToRegisterWithPasswordResetError(string email)
        {
            TempData["EmailActionError"] = $"Impossible to reset password for '{email}'.<strong> Register again.</strong>";
            return RedirectToAction("Register", "Account");
        }
        #endregion
    }
}
