using HtmlAgilityPack;
using System.Net.Http.Headers;

namespace IssueTracker.IntegrationTests.Library.Helpers
{
    public static class FormHelpers
    {
        public static async Task<HttpResponseMessage> SendFormAsync(this HttpClient client, HttpMethod method, string formUri, object model)
        {
            return await client.SendFormAsync(method, formUri, formUri, model);
        }

        public static async Task<HttpResponseMessage> SendFormAsync(this HttpClient client, HttpMethod method, string getUri, string postUri, object model)
        {
            var page = await client.GetAsync(getUri);

            var pageHtml = await page.Content.ReadAsStringAsync();

            var cookieToken = ExtractCookieToken(page.Headers);
            var formToken = ExtractFormToken(pageHtml, "AntiForgeryField");

            var request = new HttpRequestMessage(method, postUri);
            var content = CreateDictionaryFromObject(model);
            content.Add("AntiForgeryField", formToken);
            request.Content = new FormUrlEncodedContent(content);
            request.Headers.Add("Cookie", $"AntiForgeryCookie={cookieToken}");

            return await client.SendAsync(request);
        }

        private static Dictionary<string, string> CreateDictionaryFromObject(object obj)
        {
            var output = new Dictionary<string, string>();
            if (obj == null)
                return output;

            var props = obj.GetType().GetProperties();

            foreach (var property in props)
            {
                var key = property.Name;
                var value = property.GetValue(obj)?.ToString();
                if (value != null)
                {
                    output.Add(key, value);
                }
            }

            return output;
        }

        private static string ExtractFormToken(string html, string fieldName)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            var verificationToken = document.DocumentNode
                .SelectSingleNode($"//input[@name='{fieldName}']")
                .GetAttributeValue<string>("value", "");

            return verificationToken;
        }

        private static string ExtractCookieToken(HttpResponseHeaders headers)
        {
            var headerValue = headers.GetValues("Set-Cookie").First();
            var cookieToken = headerValue.Split(';').First().Split('=').Last();

            return cookieToken;
        }
    }
}
