using IssueTracker.Domain.Constants;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Enums;
using IssueTracker.UI.Models.Issues;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace IssueTracker.UI.FunctionalTests.Views.Issues
{
    public class EditViewTests : UiTestsFixture
    {
        private Project _project;
        private int _issueId;
        private string URI { get => $"/Projects/{_project.Id}/Issues/{_issueId}/Edit"; }
        private string FORM_ACTION { get => $"action=\"{URI}\""; }
        public EditViewTests() : base()
        {
            AuthenticateFactory(new List<Claim>()
            {
                new Claim(AppClaimTypes.ProjectAccess, "1")
            });
            _project = SetupTestProjectAsync("Test Project").GetAwaiter().GetResult();
            _issueId = _project.Issues.First().Id;
        }

        [Fact]
        public async Task Get_WhenUserAuthenticated_ShouldReturnViewWithFormToCreateIssue()
        {
            //Act
            var page = await Client.GetAsync(URI);
            var pageHtml = await page.Content.ReadAsStringAsync();

            //Assert
            page.StatusCode.Should().Be(HttpStatusCode.OK);
            pageHtml.Should().Contain(FORM_ACTION);
        }

        [Fact]
        public async Task Post_WhenEnteredDataIsValid_ShouldUpdateIssueInDatabaseAndRedirectToDetailsAction()
        {
            //Arrange
            var model = new EditIssueViewModel()
            {
                Id = _issueId,
                ProjectId = _project.Id,
                Title = nameof(Post_WhenEnteredDataIsValid_ShouldUpdateIssueInDatabaseAndRedirectToDetailsAction),
                Description = "Description",
                Priority = PriorityLevel.Low,
                Status = WorkingStatus.Completed
            };

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, URI, model);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            response.Headers.Location?.OriginalString.Should().Contain("Details");
            var addedIssue = Database.Func(ctx => ctx.Issues.Include(x => x.Members).First(x => x.Title == model.Title));
            addedIssue.Should().NotBeNull();
            addedIssue.Title.Should().Be(model.Title);
            addedIssue.Description.Should().Be(model.Description);
            addedIssue.Priority.Should().Be(model.Priority);
            addedIssue.Status.Should().Be(model.Status);
        }

        [Fact]
        public async Task Post_WhenIssueIsUpdatedInProjectWithoutMembership_ShouldReturnForbidden()
        {
            //Arrange
            var projectWithoutMembership = await SetupTestProjectAsync(nameof(Post_WhenIssueIsUpdatedInProjectWithoutMembership_ShouldReturnForbidden),
                false);
            var model = new EditIssueViewModel()
            {
                Id = projectWithoutMembership.Issues.First().Id,
                ProjectId = projectWithoutMembership.Id,
                Title = nameof(Post_WhenIssueIsUpdatedInProjectWithoutMembership_ShouldReturnForbidden),
                Description = "Description",
                Priority = PriorityLevel.Low,
                Status = WorkingStatus.Completed
            };

            //Act
            var response = await Client.SendFormAsync(HttpMethod.Post, URI, model);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
