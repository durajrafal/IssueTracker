﻿using IssueTracker.Application.Projects.Queries.GetProjects;
using IssueTracker.UI.Models;
using IssueTracker.UI.Models.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IssueTracker.UI.Controllers
{
    
    public class HomeController : ControllerWithMediatR
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = new GetProjects();
            var result = await Mediator.Send(query);
            var vm = new ProjectsSummaryListViewModel() { Projects = result.ToList() };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}