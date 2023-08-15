using FluentValidation.TestHelper;
using IssueTracker.Application.Projects.Commands.UpdateProject;

namespace IssueTracker.Application.UnitTests.Validators.Project
{
    public class UpdateProjectValidatorTests
    {
        private Mock<IApplicationDbContext> _mockCtx = new();
        private readonly UpdateProjectValidator _validator;
        const string PROJECT_NAME = "Test Project";

        public UpdateProjectValidatorTests()
        {
            _validator = new(_mockCtx.Object);
        }

        [Fact]
        public void Validate_WhenTitleIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateProject { Title = String.Empty };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>());
            _mockCtx.Setup(x => x.Projects).Returns(mockSet.Object);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_WhenTitleIsTooLong_ShouldHaveValidationError()
        {
            var command = new UpdateProject
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
            var command = new UpdateProject
            {
                Id = 2,
                Title = PROJECT_NAME
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Domain.Entities.Project>
            {
                new Domain.Entities.Project
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
            var command = new UpdateProject
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
