using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace IssueTracker.UI.TagHelpers
{
    [HtmlTargetElement(Attributes = "app-invalid-if-errors")]
    public class InvalidIfErrorsTagHelper : TagHelper
    {
        [HtmlAttributeName("app-invalid-if-errors")]
        public ModelExpression For { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var entry) && entry.Errors.Count > 0)
            {
                if (output.Attributes.FirstOrDefault(x => x.Name == "class") != null)
                    output.AddClass("is-invalid", HtmlEncoder.Default);
                else
                    output.Attributes.Add("class", "is-invalid");
            }
        }
    }
}
