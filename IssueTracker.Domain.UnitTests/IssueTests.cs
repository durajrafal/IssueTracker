using IssueTracker.Domain.ValueObjects;

namespace IssueTracker.Domain.UnitTests
{
    public class IssueTests
    {
        private Issue Issue;
        private Member Member;
        public IssueTests()
        {
            Issue = new Issue();
            var guid = Guid.NewGuid().ToString();
            Member = new Member()
            {
                UserId = guid,
                User = new User()
                {
                    UserId = guid,
                    FirstName = "First",
                    LastName = "Last"
                }
            };
        }

        [Fact]
        public void UpdateMembers_WhenMembersAdded_ShouldCreateAuditEventForMembersCollectionAddition()
        {
            //Act
            Issue.UpdateMembers(new List<Member> { Member }, Member.UserId);

            //Assert
            Issue.AuditEvents.Should().HaveCount(1);
            Issue.AuditEvents.First().GetSummaryText()
                .Should().Contain(Member.User.FullName)
                .And.Contain(CollectionOperation.Added.ToString());
        }

        [Fact]
        public void UpdateMembers_WhenMembersRemoved_ShouldCreateAuditEventForMembersCollectionAddition()
        {
            //Arrange
            Issue.Members.Add(Member);

            //Act
            Issue.UpdateMembers(new List<Member>(), Member.UserId);

            //Assert
            Issue.AuditEvents.Should().HaveCount(1);
            Issue.AuditEvents.First().GetSummaryText()
                .Should().Contain(Member.User.FullName)
                .And.Contain(CollectionOperation.Removed.ToString());
        }
    }
}
