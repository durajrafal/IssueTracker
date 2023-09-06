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
            var users = _userService.GetAllUsers();
            var vm = users.Select(x => new UserAdminViewModel
            {
                UserId = x.UserId,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                RoleClaim = _userService.GetUserRoleClaimAsync(x.UserId).GetAwaiter().GetResult().Value,
            });
            return View(vm);
        } 

        [HttpGet]
        public async Task<IActionResult> GetUserClaims(string id)
        {
            var roleClaim = await _userService.GetUserRoleClaimAsync(id);
            var user = await _userService.GetUserByIdAsync(id);

            if (roleClaim is null || user is null)
                return BadRequest();

            var vm = new UpdateUserRoleClaimViewModel(roleClaim, user);
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
