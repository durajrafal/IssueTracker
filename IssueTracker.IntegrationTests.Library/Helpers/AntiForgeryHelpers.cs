using HtmlAgilityPack;
using System.Net.Http.Headers;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class AntiForgeryHelpers
    {
        public static string ExtractFormToken(string html, string fieldName)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var verificationToken = document.DocumentNode
                .SelectSingleNode($"//input[@name='{fieldName}']")
                .GetAttributeValue<string>("value", "");

            return verificationToken;
        }

        public static string ExtractCookieToken(HttpResponseHeaders headers)
        {
            var headerValue = headers.GetValues("Set-Cookie").First();
            var cookieToken = headerValue.Split(';').First().Split('=').Last();

            return cookieToken;
        }
    }
}
