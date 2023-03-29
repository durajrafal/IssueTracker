using Microsoft.AspNetCore.Html;

namespace IssueTracker.UI.Models
{
    public abstract class CustomErrorMessageModel
    {
        public HtmlString? ErrorMessage { get; set; }
    }
}
