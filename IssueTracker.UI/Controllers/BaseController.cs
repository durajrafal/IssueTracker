using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected ISender Mediator => HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
