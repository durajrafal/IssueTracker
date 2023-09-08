using IssueTracker.Application.Common.Helpers;
using IssueTracker.Domain.Entities;
using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Application.UnitTests.Extensions
{
    public class MembersExtensionsTests
    {
        public MembersExtensionsTests()
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
            var members = new List<Member>();

            //Act
            members.AddNewOrExistingMember(mockSet.Object, existingMember.UserId);

            //Assert
            members.First().Id.Should().Be(existingMember.Id);


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
            var members = new List<Member>();

            //Act
            members.AddNewOrExistingMember(mockSet.Object, "otherUserId");

            //Assert
            members.First().Id.Should().NotBe(existingMember.Id);
        }

        [Fact]
        public async Task SyncMembersWithUsers_WhenUserExists_ShouldPopulateMemberWithIt()
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
            await members.SyncMembersWithUsers(mockUserService.Object);

            //Arrange
            members.First().User.FirstName.Should().Be(user.FirstName);
            members.First().User.LastName.Should().Be(user.LastName);
            members.First().User.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task SyncMembersWithUser_WhenUserDoesNotExists_ShouldDeleteMemberWithoutExistingUser()
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
            await members.SyncMembersWithUsers(mockUserService.Object);

            //Assert
            members.FirstOrDefault(x => x.Id == 2).Should().BeNull();
            members.Count.Should().Be(1);
        }
    }
}
