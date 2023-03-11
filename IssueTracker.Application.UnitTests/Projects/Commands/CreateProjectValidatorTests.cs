using FluentValidation.TestHelper;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.UnitTests.Common;
using IssueTracker.Domain.Models;
using Moq;

namespace IssueTracker.Application.UnitTests.Projects.Commands
{
    public class CreateProjectValidatorTests
    {
        Mock<IApplicationDbContext> _mockCtx = new();
        CreateProjectCommandValidator _validator;
        const string PROJECT_NAME = "Test Project";

        public CreateProjectValidatorTests()
        {
            _validator = new(_mockCtx.Object);
        }

        [Fact]
        public void Validate_TitleIsEmpty_ShouldHaveValidationError()
        {
            var command = new CreateProjectCommand();
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_TitleIsTooLong_ShouldHaveValidationError()
        {
            var command = new CreateProjectCommand
            {
                Title = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_TitleIsNotUnique_ShouldHaveValidationError()
        {
            var command = new CreateProjectCommand
            {
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>
            {
                new Project
                {
                    Title = PROJECT_NAME 
                } 
            });
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_TitleIsOk_ShouldNotHaveValidationError()
        {
            var command = new CreateProjectCommand
            {
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>
            {
                new Project
                {
                    Title = "Another test project"
                }
            });
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

    }
}
