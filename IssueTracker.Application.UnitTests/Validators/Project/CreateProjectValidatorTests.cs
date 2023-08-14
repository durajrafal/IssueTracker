using FluentValidation.TestHelper;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.UnitTests.Common;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace IssueTracker.Application.UnitTests.Validators.Project
{
    public class CreateProjectValidatorTests
    {
        private Mock<IApplicationDbContext> _mockCtx = new();
        private readonly CreateProjectValidator _validator;
        const string PROJECT_NAME = "Test Project";

        public CreateProjectValidatorTests()
        {
            _validator = new(_mockCtx.Object);
        }

        [Fact]
        public void Validate_WhenTitleIsEmpty_ShouldHaveValidationError()
        {
            var command = new CreateProject();
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsTooLong_ShouldHaveValidationError()
        {
            var command = new CreateProject
            {
                Title = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsNotUnique_ShouldHaveValidationError()
        {
            var command = new CreateProject
            {
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>
            {
                new Domain.Entities.Project
                {
                    Title = PROJECT_NAME
                }
            });
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsOk_ShouldNotHaveValidationError()
        {
            var command = new CreateProject
            {
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>
            {
                new Domain.Entities.Project
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
