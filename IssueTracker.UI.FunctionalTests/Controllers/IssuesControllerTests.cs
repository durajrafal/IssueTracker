using IssueTracker.Domain.Entities;
using IssueTracker.UI.Controllers;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.FunctionalTests.Controllers
{
    public class IssuesControllerTests : UiTestsFixture
    {
        private readonly IssuesController _controller;
        public IssuesControllerTests()
        {
            AuthenticateFactory();
            _controller = CreateControllerWithContext<IssuesController>();
        }

        [Fact]
        public async Task GetCreate_WhenProjectIdIsValid_ShouldReturnViewWithSeededModel()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(GetCreate_WhenProjectIdIsValid_ShouldReturnViewWithSeededModel));

            //Act
            var response = await _controller.Create(project.Id) as ViewResult;
            var model = response.Model as CreateIssueViewModel;

            //Assert
            model.ProjectMembersSelecList.Should().HaveCount(project.Members.Count);
            model.ProjectId.Should().Be(project.Id);
            response.ViewData.ModelState.Values.Should().BeEmpty();

        }

        [Fact]
        public async Task PostCreate_WhenDataIsInvalid_ShoulReturnViewWithSeededModelAndValidationErrors()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(PostCreate_WhenDataIsInvalid_ShoulReturnViewWithSeededModelAndValidationErrors));
            var responseGet = await _controller.Create(project.Id) as ViewResult;
            var model = responseGet.Model as CreateIssueViewModel;

            //Act
            var responsePost = await _controller.Create(model) as ViewResult;

            //Assert
            var responseModel = responsePost.Model as CreateIssueViewModel;
            responseModel.ProjectMembersSelecList.Should().NotBeEmpty();
            responsePost.ViewData.ModelState.ErrorCount.Should().BeGreaterThan(0);
            responsePost.ViewData.ModelState.Values.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetDetails_WhenIdIsValid_ShoulReturnViewWithSeededModel()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(PostCreate_WhenDataIsInvalid_ShoulReturnViewWithSeededModelAndValidationErrors));
            var issue = project.Issues.First();

            //Act
            var response = await _controller.Details(issue.Id) as ViewResult;
            var responseModel = response!.Model as IssueViewModel;

            //Assert
            responseModel.Id.Should().Be(issue.Id);
            responseModel.Title.Should().Be(issue.Title);
            responseModel.Description.Should().Be(issue.Description);
            responseModel.Priority.Should().Be(issue.Priority);
            responseModel.Status.Should().Be(issue.Status);
            responseModel.Project.Should().NotBeNull();
            responseModel.Members.Should().NotBeEmpty();
            responseModel.Members.Should().AllSatisfy(x => x.User.Should().NotBeNull());
        }

        [Fact]
        public async Task GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),
                false);
            var issue = project.Issues.First();

            //Act
            Func<Task> act = async () => await _controller.Details(issue.Id);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
