using IssueTracker.Application.Common.Interfaces;
using IssueTracker.UI.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Policy = "UserAdministration")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            var vm = users.Select(x => new UserAdminViewModel
            {
                UserId = x.UserId,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                RoleClaim = x.RoleClaim,
            });
            return View(vm);
        } 

        [HttpGet]
        public async Task<IActionResult> GetUserClaims(string id)
        {
            var claims = await _userService.GetUserClaimsAsync(id);
            var vm = new UpdateUserRoleClaimViewModel(claims, id);
            return PartialView("../Admin/_UserClaims", vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRoleClaim(UpdateUserRoleClaimViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await _userService.ChangeUserRoleClaimAsync(vm.UserId, vm.SelectedRole);
            }
            return RedirectToAction("Users");
        }
    }
}
