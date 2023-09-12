using IssueTracker.Domain.Enums;
using IssueTracker.UI.Controllers;
using IssueTracker.UI.Models.Issues;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var response = await _controller.Details(issue.Id, null) as ViewResult;
            var responseModel = response!.Model as IssueViewModel;

            //Assert
            responseModel.Id.Should().Be(issue.Id);
            responseModel.Title.Should().Be(issue.Title);
            responseModel.Description.Should().Be(issue.Description);
            responseModel.Priority.Should().Be(issue.Priority);
            responseModel.Status.Should().Be(issue.Status);
            responseModel.Project.Should().NotBeNull();
            responseModel.Project.Members.Should().HaveCount(project.Members.Count)
                .And.AllSatisfy(x => x.User.Should().NotBeNull());
            responseModel.Members.Should().HaveCount(issue.Members.Count)
                .And.AllSatisfy(x => x.User.Should().NotBeNull());
            responseModel.Audit.AuditEvents.PageNumber.Should().Be(1);
            responseModel.Audit.AuditEvents.PageSize.Should().Be(5);
        }

        [Fact]
        public async Task GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(nameof(GetDetails_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException),
                false);
            var issue = project.Issues.First();

            //Act
            Func<Task> act = async () => await _controller.Details(issue.Id, null);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task GetEdit_WhenDataIsValid_ShoulReturnViewWithSeededModel()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var response = await _controller.Edit(issue.Id) as ViewResult;
            var responseModel = response!.Model as EditIssueViewModel;

            //Assert
            responseModel.Id.Should().Be(issue.Id);
            responseModel.Title.Should().Be(issue.Title);
            responseModel.Description.Should().Be(issue.Description);
            responseModel.Priority.Should().Be(issue.Priority);
            responseModel.Status.Should().Be(issue.Status);
            responseModel.ProjectId.Should().Be(issue.Project.Id);
        }

        [Fact]
        public async Task GetEdit_WhenCurrentUserIsNotMember_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var project = await SetupTestProjectAsync(addCurrentUser: false);
            var issue = project.Issues.First();

            //Act
            Func<Task> act = async () => await _controller.Edit(issue.Id);

            //Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task PostEdit_WhenTitleIsValidAndNotEmpty_ShouldUpdateIssueInDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var vm = new EditIssueViewModel()
            {
                Id = issue.Id,
                ProjectId = project.Id,
                Title = "Updated Title",
                Description = "Updated description",
                Priority = PriorityLevel.Low,
                Status = WorkingStatus.Completed
            };

            //TODO - find a way to create IUrlHelper so RedirectToAction don't throw exception
            try
            {
                var response = await _controller.Edit(vm.Id, vm);
            }
            catch (Exception e)
            {
                if (e is not NullReferenceException)
                    throw;
            }

            //Assert
            var updatedIssue = Database.Func(ctx => ctx.Issues.First(x => x.Id == vm.Id));
            updatedIssue.Title.Should().Be(vm.Title);
            updatedIssue.Description.Should().Be(vm.Description);
            updatedIssue.Priority.Should().Be(vm.Priority);
            updatedIssue.Status.Should().Be(vm.Status);
        }

        [Fact]
        public async Task DeleteDelete_WhenIdIsValid_ShouldReturnNoContentAndDeleteIssueFromDatabase()
        {
            //Arrange
            var project = await SetupTestProjectAsync();
            var issue = project.Issues.First();

            //Act
            var response = await _controller.Delete(issue.Id) as NoContent;

            //Assert
            response.Should().NotBeNull();
            Database.Func(ctx => ctx.Issues.FirstOrDefault(x => x.Id == issue.Id)).Should().BeNull();
        }

        [Fact]
        public async Task DeleteDelete_WhenIdIsInvalid_ShouldReturnNotFound()
        {
           //Act
            var response = await _controller.Delete(0) as NotFound<string>;

            //Assert
            response.Should().NotBeNull();
            response.Value.Should().Contain("Issue").And.Contain("0");
        }

        [Fact]
        public async Task DeleteDelete_WhenCurrentUserIsNotMember_ShouldReturnForbidAndDoNotDeleteProjectFromDatabas()
        {
            //Arrange
            var project = await SetupTestProjectAsync(addCurrentUser:false);
            var issue = project.Issues.First();

            //Act
            var response = await _controller.Delete(issue.Id) as ForbidHttpResult;

            //Assert
            response.Should().NotBeNull();
            Database.Func(ctx => ctx.Issues.FirstOrDefault(x => x.Id == issue.Id)).Should().NotBeNull();
        }
    }
}
