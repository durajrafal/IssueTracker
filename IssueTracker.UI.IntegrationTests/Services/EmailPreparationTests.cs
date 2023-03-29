using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.UI.IntegrationTests.Services
{
    public class EmailPreparationTests : BaseTestWithScope
    {
        public EmailPreparationTests(CustomWebApplicationFactory factory)
            :base(factory)
        {

        }

        [Fact]
        public void GetConfirmationEmailBody_Always_IsHtmlAndContainsToken()
        {
            var token = Guid.NewGuid().ToString();
            var service = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailPreparationService>();

            var body = service.GetConfirmationEmailBody(token);

            Assert.Contains("<!DOCTYPE html>", body);
            Assert.Contains(token, body);
        }
    }
}
