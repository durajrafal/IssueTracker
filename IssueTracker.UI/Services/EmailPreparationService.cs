using IssueTracker.Application.Common.Interfaces;
using System.Web;

namespace IssueTracker.UI.Services
{
    public class EmailPreparationService : IEmailPreparationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _webRootPath => _webHostEnvironment.WebRootPath;
        private const string TEMPLATES_PATH = "email_templates";
        private const string MAIN_LAYOUT = "_layout.html";
        private const string CONFIRMATION_CONTENT = "confirmation.html";
        public EmailPreparationService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetConfirmationEmailBody(string token)
        {
            var layout = GetTemplate(MAIN_LAYOUT);
            var content = GetTemplate(CONFIRMATION_CONTENT);
            content = content.Replace("#token", token);
            return layout.Replace("#EmailContent", content);
        }

        private string GetTemplate(string templateName)
        {
            var path = Path.Combine(_webRootPath, TEMPLATES_PATH, templateName);
            var template = File.ReadAllText(path);
            return template;
        }
    }
}
