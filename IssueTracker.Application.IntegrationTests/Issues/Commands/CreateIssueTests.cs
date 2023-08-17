﻿using IssueTracker.Application.Issues.Commands;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using IssueTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Domain.Enums;

namespace IssueTracker.Application.IntegrationTests.Issues.Commands
{
    public class CreateIssueTests : TestsWithMediatorFixture
    {
        public CreateIssueTests(): base()
        {

        }

        [Fact]
        public async Task Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var command = new CreateIssue { Title = nameof(Handle_WhenTitleNotEmptyAndUnique_ShouldAddToDatabase), ProjectId = project.Id};
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.First(x => x.Id == addedIssueId));
            addedIssue.Title.Should().Be(command.Title);
        }

        [Fact]
        public async Task Handle_WhenTitleIsEmpty_ShouldThrowValidationException()
        {
            //Act
            var command = new CreateIssue();
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenTitleIsNotUniqueWithinTheProject_ShouldThrowValidationException()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenTitleIsNotUniqueWithinTheProject_ShouldThrowValidationException));
            project.Issues.Add(new Issue { Title = "Not unique issue title" });
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var command = new CreateIssue { Title = "Not unique issue title", ProjectId = project.Id };
            var act = async () => await Mediator.Send(command);

            //Assert
            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject()
        {

            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var command = new CreateIssue { Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldBeConnectedToParentProject), ProjectId = project.Id };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var parentProject = Database.Func(ctx => ctx.Projects.Include(x => x.Issues).First(x => x.Id == project.Id));
            var issueInParentProject = parentProject.Issues.FirstOrDefault(x => x.Id == addedIssueId);
            issueInParentProject.Should().NotBeNull();
            issueInParentProject?.Title.Should().Be(command.Title);
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });
            var member = project.Members.First();

            //Act
            var command = new CreateIssue
            {
                Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveAllDataSaved),
                ProjectId = project.Id,
                Description = "My project description",
                Priority = PriorityLevel.High,
                Members = new List<Member> { new Member() { UserId= Guid.NewGuid().ToString() }, member }
            };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.Include(x => x.Members).First(x => x.Id == addedIssueId));
            addedIssue.Title.Should().Be(command.Title);
            addedIssue.Description.Should().Be(command.Description);
            addedIssue.Priority.Should().Be(command.Priority);
            addedIssue.Members.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault()
        {
            //Arrange
            var project = ProjectHelpers.CreateTestProject(nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault));
            await Database.ActionAsync(async ctx =>
            {
                await ctx.Projects.AddAsync(project);
            });

            //Act
            var command = new CreateIssue
            {
                Title = nameof(Handle_WhenIssueIsAddedToDatabase_ShouldHaveCorrectEnumsValuesByDefault),
                ProjectId = project.Id,
            };
            var addedIssueId = await Mediator.Send(command);

            //Assert
            var addedIssue = Database.Func(ctx => ctx.Issues.First(x => x.Id == addedIssueId));
            addedIssue.Priority.Should().Be(PriorityLevel.None);
            addedIssue.Status.Should().Be(WorkingStatus.Pending);
        }
    }
}