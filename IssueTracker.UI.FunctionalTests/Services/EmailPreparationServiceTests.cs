﻿using IssueTracker.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IssueTracker.UI.FunctionalTests.Services
{
    public class EmailPreparationServiceTests : UiTestsFixture
    {
        public EmailPreparationServiceTests() : base()
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
