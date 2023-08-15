using FluentAssertions;
using FluentAssertions.Execution;
using IssueTracker.Application.Common.Helpers;
using IssueTracker.Application.Common.Interfaces;
using IssueTracker.Application.UnitTests.Common;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.Models;
using Moq;

namespace IssueTracker.Application.UnitTests.Extensions
{
    public class ProjectExtensionsTests
    {
        public ProjectExtensionsTests()
        {

        }

        [Fact]
        public async Task AddNewOrExistingMember_WhenMemberExists_ShouldAddExistingMember()
        {
            //Arrange
            var existingMember = new Member()
            {
                Id = 1,
                UserId = "abcd"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Member>() { existingMember });
            var project = new Project();

            //Act
            project.AddNewOrExistingMember(mockSet.Object, existingMember.UserId);

            //Assert
            project.Members.First().Id.Should().Be(existingMember.Id);


        }

        [Fact]
        public void AddNewOrExistingMember_WhenMemberDoesNotExist_ShouldAddNewMember()
        {
            //Arrange
            var existingMember = new Member()
            {
                Id = 1,
                UserId = "abcd"
            };
            var mockSet = MockingEF.CreateFakeDbSet(new List<Member>() { existingMember });
            var project = new Project();

            //Act
            project.AddNewOrExistingMember(mockSet.Object, "otherUserId");

            //Assert
            project.Members.First().Id.Should().NotBe(existingMember.Id);
        }

        [Fact]
        public async Task PopulateMembersWithUsers_Always_ShouldPopulateExistingUsers()
        {
            //Arrange
            var userId = "abcd";
            var user = new User()
            {
                UserId = userId,
                FirstName = "Joe",
                LastName = "Doe",
                Email = "joe@doe.com"
            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).Returns(Task.FromResult(user));
            var members = new List<Member>() {
                new Member()
                {
                    Id = 1,
                    UserId = userId
                }
            };

            //Act
            await members.PopulateMembersWithUsersAsync(mockUserService.Object);

            //Arrange
            members.First().User.FirstName.Should().Be(user.FirstName);
            members.First().User.LastName.Should().Be(user.LastName);
            members.First().User.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task PopulateMembersWithUsers_Always_ShouldLeaveNotExistingUsersAsNull()
        {
            //Arrange
            var userId = "abcd";
            var user = new User()
            {
                UserId = userId,
                FirstName = "Joe",
                LastName = "Doe",
                Email = "joe@doe.com"
            };
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            mockUserService.Setup(x => x.GetUserByIdAsync("deletedUser")).ReturnsAsync((User)null);
            var members = new List<Member>() {
                new Member()
                {
                    Id = 1,
                    UserId = userId
                },
                new Member()
                {
                    Id = 2,
                    UserId = "deletedUser"
                }
            };

            //Act
            await members.PopulateMembersWithUsersAsync(mockUserService.Object);

            //Assert
            members.First(x => x.Id == 2).User.Should().BeNull();
        }
    }
}
