﻿using IssueTracker.Application.Common.Interfaces;
using IssueTracker.UI.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Areas.Identity.Controllers
{
    [Area("Identity")]

    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Policy = "UserAdministration")]
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        } 
        
        [Authorize(Policy = "UserAdministration")]
        [HttpGet]
        public async Task<IActionResult> GetUserClaims(string id)
        {
            var claims = await _userService.GetUserClaimsAsync(id);
            var vm = new UpdateUserRoleClaimViewModel(claims, id);
            return PartialView("../Admin/_UserClaims", vm);
        }

        [Authorize(Policy = "UserAdministration")]
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
