using FluentValidation.TestHelper;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.DeleteProject;
using IssueTracker.Application.UnitTests.Common;
using IssueTracker.Domain.Entities;
using Moq;

namespace IssueTracker.Application.UnitTests.Projects.Commands
{
    public class DeleteProjectValidatorTests
    {
        private Mock<IApplicationDbContext> _mockCtx = new();
        private readonly DeleteProjectCommandValidator _validator;
        const string PROJECT_NAME = "Test Project";

        public DeleteProjectValidatorTests()
        {
            _validator = new(_mockCtx.Object);
        }

        [Fact]
        public void Validate_WhenTitleMatch_ShouldNotHaveValidationError()
        {
            var command = new DeleteProjectCommand
            {
                ProjectId = 1,
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Title = PROJECT_NAME,
                }
            });
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleDoesNotMatch_ShouldHaveValidationError()
        {
            var command = new DeleteProjectCommand
            {
                ProjectId = 1,
                Title = PROJECT_NAME + "s"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>
            {
                new Project
                {
                    Id = 1,
                    Title = PROJECT_NAME,
                }
            });
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
    }
}
