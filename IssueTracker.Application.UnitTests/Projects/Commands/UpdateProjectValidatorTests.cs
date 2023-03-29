using FluentValidation.TestHelper;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.Projects.Commands.CreateProject;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.UnitTests.Common;
using IssueTracker.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.UnitTests.Projects.Commands
{
    public class UpdateProjectValidatorTests
    {
        private Mock<IApplicationDbContext> _mockCtx = new();
        private readonly UpdateProjectCommandValidator _validator;
        const string PROJECT_NAME = "Test Project";

        public UpdateProjectValidatorTests()
        {
            _validator = new(_mockCtx.Object);
        }

        [Fact]
        public void Validate_WhenTitleIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateProjectCommand { Title = String.Empty };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsTooLong_ShouldHaveValidationError()
        {
            var command = new UpdateProjectCommand
            {
                Title = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsNotUnique_ShouldHaveValidationError()
        {
            var command = new UpdateProjectCommand
            {
                ProjectId = 2,
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

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsOk_ShouldNotHaveValidationError()
        {
            var command = new UpdateProjectCommand
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
