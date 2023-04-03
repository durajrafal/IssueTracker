using IssueTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailPreparationService _emailPreparation;

        public EmailService(IConfiguration configuration, IEmailPreparationService emailPreparation)
        {
            _configuration = configuration;
            _emailPreparation = emailPreparation;
        }

        public async Task<bool> SendConfirmationEmailAsync(string email, string name, string confirmationLink)
        {
            var body = _emailPreparation.GetConfirmationEmailBody(confirmationLink);
            return await SendEmail(email, name, body);
        }

        public async Task<bool> SendResetPasswordEmailAsync(string email, string name, string resetLink)
        {
            var body = _emailPreparation.GetResetPasswordEmailBody(resetLink);
            return await SendEmail(email, name, body);
        }

        private async Task<bool> SendEmail(string email, string name, string body)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_configuration["SendGrid:Email"], _configuration["SendGrid:Name"]),
                Subject = "Confirm your email",
                HtmlContent = body,
            };
            msg.AddTo(new EmailAddress(email, name));
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
    }
}
