using FluentValidation;
using IssueTracker.Application.Common.Exceptions;
using IssueTracker.Application.IntegrationTests.Projects.Queries;
using IssueTracker.Application.Projects.Commands.UpdateProject;
using IssueTracker.Application.Projects.Queries.GetProjectDetails;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IssueTracker.Application.IntegrationTests.Projects.Commands
{
    public class UpdateProjectTests : ApplicationBaseTest
    {
        public UpdateProjectTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException()
        {
            var command = new UpdateProjectCommand { ProjectId = 0 , Title = nameof(Handle_WhenProjectIdIsInvalid_ThrowsNotFoundException) };

            await Assert.ThrowsAsync<NotFoundException>(() => Mediator.Send(command));
        }

        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldAddNewUser));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            var projectMembersStartCount = project.Members.Count;
            project.Members.Add(new ProjectMember { UserId = Guid.NewGuid().ToString() });
            IList<ProjectMember> members = project.Members;

            var command = new UpdateProjectCommand 
            { 
                ProjectId = project.Id, 
                Title = project.Title,
                Members = members 
            };
            await Mediator.Send(command);

            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount + 1, udpatedProject.Members.Count);
        }
        
        [Fact]
        public async Task Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser()
        {
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenProjectIdIsValidAndTitleUnique_ShouldDeleteUser));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            var projectMembersStartCount = project.Members.Count;
            var projectUser = project.Members.First();
            project.Members.Remove(projectUser);
            IList<ProjectMember> members = project.Members;

            var command = new UpdateProjectCommand 
            { 
                ProjectId = project.Id, 
                Title = project.Title,
                Members = members
            };
            await Mediator.Send(command);

            var udpatedProject = Database.Func(ctx => 
                ctx.Projects.Include(x => x.Members).First(x => x.Id == project.Id)
            );
            Assert.Equal(projectMembersStartCount - 1, udpatedProject.Members.Count);
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUnique_ThrowsValidationException()
        {
            var project = new Project { Title = nameof(Handle_WhenTitleIsNotUnique_ThrowsValidationException) };
            string takenTitle = "Already taken title";
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
                await ctx.Projects.AddAsync(new Project { Title = takenTitle });
            });

            var command = new UpdateProjectCommand { ProjectId = project.Id, Title = takenTitle };

            await Assert.ThrowsAsync<ValidationException>(() => Mediator.Send(command));
        }
    }
}
