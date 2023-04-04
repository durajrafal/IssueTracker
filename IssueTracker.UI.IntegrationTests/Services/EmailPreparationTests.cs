using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Services
{
    public class EmailPreparationTests : BaseTest
    {
        public EmailPreparationTests(CustomWebApplicationFactory factory)
            :base(factory)
        {

        }

        [Fact]
        public void GetConfirmationEmailBody_Always_ShouldBeFormattedAsHtmlAndContainsLink()
        {
            var link = Guid.NewGuid().ToString();
            var service = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailPreparationService>();

            var body = service.GetConfirmationEmailBody(link);

            Assert.Contains("<!DOCTYPE html>", body);
            Assert.Contains(link, body);
        }

        [Fact]
        public void GetResetPasswordEmailBody_Always_ShouldBeFormattedAsHtmlAndContainsLink()
        {
            var link = Guid.NewGuid().ToString();
            var service = ScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailPreparationService>();

            var body = service.GetResetPasswordEmailBody(link);

            Assert.Contains("<!DOCTYPE html>", body);
            Assert.Contains(link, body);
        }
    }
}
