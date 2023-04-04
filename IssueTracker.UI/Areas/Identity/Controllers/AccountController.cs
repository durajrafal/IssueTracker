using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace IssueTracker.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, [FromServices] SignInManager<ApplicationUser> signInManager)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                       userName: login.Email,
                       password: login.Password,
                       isPersistent: login.IsRememberChecked,
                       lockoutOnFailure: false);

                return ResolveSignInResult(login, result);
            }

            return View(login);
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsDeveloper([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("dev@test.com", "Pass123", false, false);
            return ResolveSignInResult(new LoginViewModel(), result);
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsManager([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("manager@test.com", "Pass123", false, false);
            return ResolveSignInResult(new LoginViewModel(), result);
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsAdmin([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("admin@test.com", "Pass123", false, false);
            return ResolveSignInResult(new LoginViewModel(), result);
        }

        private IActionResult ResolveSignInResult(LoginViewModel login, Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            else if (result.IsNotAllowed)
            {
                login.ErrorMessage = new("<strong> Email not confirmed! </strong><br> Please confirm your email and try again.");
                return View("Login", login);
            }
            else
            {
                login.ErrorMessage = new("<strong> Logging in failed! </strong><br> Check your credentials and try again.");
                return View("Login", login);
            }
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register, [FromServices] IEmailService emailService)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser(register.Email, register.FirstName, register.LastName);
                var result = await _userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("Confirm", "Email", new { token, email = user.Email }, Request.Scheme);
                    bool emailResponse = await emailService.SendConfirmationEmailAsync(user.Email, user.FullName, confirmationLink);

                    if (emailResponse)
                    {
                        var html = "Email has been sent. Please check your inbox (or <em>Spam</em> folder) to confirm your account.";
                        TempData["EmailActionSuccess"] = html;
                        return View("Login");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                        register.ErrorMessage = new HtmlString($"Failed to send an email to <strong>{register.Email}</strong>");
                        return View(register);
                    }
                }
                if (result.Errors.Count() > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var error in result.Errors.Where(x => x.Code != "DuplicateUserName"))
                    {
                        sb.Append(error.Description);
                        sb.Append("<br>");
                    }
                    register.ErrorMessage = new HtmlString(sb.ToString());

                }
            }
            return View(register);
        }
        #endregion

        #region ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm, [FromServices] IEmailService emailService)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user == null)
                    return RedirectAfterSendingResetPasswordEmail();

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Email", new { token, email = user.Email }, Request.Scheme);

                var emailResponse = await emailService.SendResetPasswordEmailAsync(user.Email, user.FullName, resetLink);

                if (emailResponse)
                {
                    return RedirectAfterSendingResetPasswordEmail();
                }
                else
                {
                    vm.ErrorMessage = new HtmlString($"Failed to send an email to <strong>{vm.Email}</strong>");
                    return View(vm);
                }
            }
            return View(vm);
        }

        private IActionResult RedirectAfterSendingResetPasswordEmail()
        {
            var html = "Email has been sent. Please check your inbox (or <em>Spam</em> folder) to reset your password.";
            TempData["EmailActionSuccess"] = html;
            return RedirectToAction("Login");
        }
        #endregion

        #region Logout
        [HttpGet]
        public async Task<IActionResult> Logout([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            await signInManager.SignOutAsync();
            return View();
        }
        #endregion

        #region Edit
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update([FromServices] ICurrentUserService currentUserService)
        {
            var userId = currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            var vm = new UpdateViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UpdateViewModel vm, [FromServices] IPasswordHasher<ApplicationUser> passwordHasher)
        {
            bool onlyPasswordInvalid = ModelState
                .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                .All(x => x.Key.Contains("Password"));
            bool modelValid = ModelState.IsValid || (onlyPasswordInvalid && !vm.PasswordChangeRequested);
            if (modelValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                bool updateRequired = false;
                if (user.FirstName != vm.FirstName)
                {
                    user.FirstName = vm.FirstName;
                    updateRequired = true;
                }
                if (user.LastName != vm.LastName)
                {
                    user.LastName = vm.LastName;
                    updateRequired = true;
                }
                if (vm.PasswordChangeRequested)
                    user.PasswordHash = passwordHasher.HashPassword(user, vm.Password);

                if (updateRequired || vm.PasswordChangeRequested)
                    await _userManager.UpdateAsync(user);
            }
            else
                return View(vm);

            return RedirectToAction("Index", "Home", new { area = ""});
        }
        #endregion
    }
}
