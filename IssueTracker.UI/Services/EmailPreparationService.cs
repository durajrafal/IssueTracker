using IssueTracker.Application.Common.Interfaces;
using Newtonsoft.Json.Linq;
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
        private const string RESET_CONTENT = "reset_password.html";
        public EmailPreparationService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string GetConfirmationEmailBody(string confirmationLink)
        {
            var layout = GetTemplate(MAIN_LAYOUT);
            var content = GetTemplate(CONFIRMATION_CONTENT);
            content = content.Replace("#link", confirmationLink);
            return layout.Replace("#EmailContent", content);
        }

        public string GetResetPasswordEmailBody(string resetLink)
        {
            var layout = GetTemplate(MAIN_LAYOUT);
            var content = GetTemplate(RESET_CONTENT);
            content = content.Replace("#link", resetLink);
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
