using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IssueTracker.UI.HtmlHelpers
{
    public static class LabelHelpers
    {
        public static IHtmlContent LabelOverLine(this IHtmlHelper helper, string text, string boostrapIconName, string target="")
        {
            var html = $"""
                <label for="{target}" class="fs-6 fst-italic bg-light px-2 py-1 ms-3 d-inline-block position-relative text-secondary fw-bold" 
                    style="top:0.5rem; z-index:1;">
                <i class="bi {boostrapIconName} me-2"></i>{text}
                </label>
                <hr class="mt-0 mb-3 ms-1 w-75" />
                """;

            return new HtmlString(html);
        }
    }
}
