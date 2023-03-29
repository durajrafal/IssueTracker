using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                        TempData["EmailConfirmSuccess"] = html;
                        return View("Login");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                        register.ResultMessage = new HtmlString($"Failed to send an email to <strong>{register.Email}</strong>");
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
                    register.ResultMessage = new HtmlString(sb.ToString());

                }
            }
            return View(register);
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
                login.ResultMessage = new("<strong> Email not confirmed! </strong><br> Please confirm your email and try again.");
                return View("Login", login);
            }
            else
            {
                login.ResultMessage = new("<strong> Logging in failed! </strong><br> Check your credentials and try again.");
                return View("Login", login);
            }
        }
    }
}
