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
    }
}
