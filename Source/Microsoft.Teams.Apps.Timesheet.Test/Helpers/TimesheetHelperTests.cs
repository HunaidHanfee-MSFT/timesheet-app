// <copyright file="TimesheetHelperTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.Test.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Teams.Apps.Timesheet.Common.Models;
    using Microsoft.Teams.Apps.Timesheet.Common.Repositories;
    using Microsoft.Teams.Apps.Timesheet.Helpers;
    using Microsoft.Teams.Apps.Timesheet.ModelMappers;
    using Microsoft.Teams.Apps.Timesheet.Models;
    using Microsoft.Teams.Apps.Timesheet.Tests.Fakes;
    using Microsoft.Teams.Apps.Timesheet.Tests.TestData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// This class lists unit test cases related to Timesheets.
    /// </summary>
    [TestClass]
    public class TimesheetHelperTests
    {
        /// <summary>
        /// Timesheet test list.
        /// </summary>
        private readonly List<TimesheetEntity> timesheets = new List<TimesheetEntity>
        {
            new TimesheetEntity
            {
                // Given same date as given in user timesheet test list.
                TimesheetDate = new DateTime(2021, 2, 1),
                TaskId = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515540e"),
                Id = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515541e"),
                Status = (int)TimesheetStatus.None,
                Task = new TaskEntity
                {
                    ProjectId = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515541e"),
                },
            },
            new TimesheetEntity
            {
                // Given same date as given in user timesheet test list.
                TimesheetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 10),
                TaskId = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515540e"),
                Id = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515541e"),
                Status = (int)TimesheetStatus.None,
                Task = new TaskEntity
                {
                    ProjectId = Guid.Parse("1eec371f-edbe-4ad1-be1d-d4cd3515541e"),
                },
            },
        };

        /// <summary>
        /// The project test data.
        /// </summary>
        private readonly List<Project> projects = new List<Project>()
        {
            new Project
            {
                Id = Guid.Parse("bfb77fc0-12a9-4250-a5fb-e52ddc48ff86"),
                Title = "TimesheetEntity App",
                ClientName = "Microsoft",
                BillableHours = 200,
                NonBillableHours = 200,
                StartDate = new DateTime(DateTime.UtcNow.Year, 1, 2),
                EndDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 28),
                Members = new List<Member>
                {
                    new Member
                    {
                        Id = Guid.Parse("d3d964ae-2979-4dac-b1e0-6c1b936c2640"),
                        ProjectId = Guid.Parse("bfb77fc0-12a9-4250-a5fb-e52ddc48ff86"),
                        UserId = Guid.Parse("e9be1d47-2707-4dfc-b2a9-e62648c3a04e"),
                        IsBillable = true,
                        IsRemoved = false,
                    },
                },
                Tasks = new List<TaskEntity>
                {
                    new TaskEntity
                    {
                        MemberMappingId = Guid.Parse("d3d964ae-2979-4dac-b1e0-6c1b936c2640"),
                        MemberMapping = new Member
                        {
                            Id = Guid.Parse("d3d964ae-2979-4dac-b1e0-6c1b936c2640"),
                            IsBillable = true,
                            IsRemoved = false,
                            UserId = Guid.Parse("d3d964ae-2979-4dac-b1e0-6c1b936c2640"),
                        },
                        Id = Guid.Parse("2dcf17b4-9bc7-488a-a59c-b0d12b14782d"),
                        ProjectId = Guid.Parse("bfb77fc0-12a9-4250-a5fb-e52ddc48ff86"),
                        IsRemoved = false,
                        Title = "Development",
                        StartDate = new DateTime(2021, 2, 1),
                        EndDate = new DateTime(2021, 2, 1),
                    },
                },
                CreatedBy = Guid.Parse("08310120-ff64-45a4-b67a-6f2f19fba937"),
                CreatedOn = DateTime.Now,
            },
        };

        /// <summary>
        /// Instance of timesheet helper.
        /// </summary>
        private TimesheetHelper timesheetHelper;

        /// <summary>
        /// The mocked instance of repository accessors to access repositories.
        /// </summary>
        private Mock<IRepositoryAccessors> repositoryAccessors;

        /// <summary>
        /// The mocked instance of timesheet repository.
        /// </summary>
        private Mock<ITimesheetRepository> timesheetRepository;

        /// <summary>
        /// Mocked instance of logger.
        /// </summary>
        private Mock<ILogger<TimesheetHelper>> logger;

        /// <summary>
        /// The instance of bot settings.
        /// </summary>
        private IOptions<BotSettings> botSettings;

        /// <summary>
        ///  Initialize all test variables.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.logger = new Mock<ILogger<TimesheetHelper>>();
            this.repositoryAccessors = new Mock<IRepositoryAccessors>();
            this.timesheetRepository = new Mock<ITimesheetRepository>();

            this.botSettings = Options.Create(new BotSettings()
            {
                MicrosoftAppId = string.Empty,
                MicrosoftAppPassword = string.Empty,
                AppBaseUri = string.Empty,
                CardCacheDurationInHour = 12,
                TimesheetFreezeDayOfMonth = 12,
                WeeklyEffortsLimit = 44,
            });
            this.timesheetHelper = new TimesheetHelper(this.botSettings, this.repositoryAccessors.Object, new TimesheetMapper(), this.logger.Object);
        }

        /// <summary>
        /// Tests whether duplicate efforts operation successful if unfrozen dates are provided.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task DuplicateEfforts_ProvideUnfrozenTargetDates_ReturnsSuccessfulOperation()
        {
            var projectRepository = new Mock<IProjectRepository>();

            projectRepository
                .Setup(x => x.GetProjectsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.projects.AsEnumerable()));

            var timesheetRepository = new Mock<ITimesheetRepository>();

            timesheetRepository
                .Setup(x => x.GetTimesheetsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.timesheets));

            this.repositoryAccessors.Setup(x => x.ProjectRepository).Returns(projectRepository.Object);
            this.repositoryAccessors.Setup(x => x.TimesheetRepository).Returns(timesheetRepository.Object);
            this.repositoryAccessors.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(2));

            var sourceDate = new DateTime(2021, 2, 1);
            var targetDates = new List<DateTime>
            {
                new DateTime(2021, 2, 5),
                new DateTime(2021, 2, 6),
            };

            var result = await this.timesheetHelper.DuplicateEffortsAsync(sourceDate, targetDates, DateTime.UtcNow, Guid.Parse("e9be1d47-2707-4dfc-b2a9-e62648c3a04e"));

            timesheetRepository.Verify(x => x.Add(It.IsAny<TimesheetEntity>()), Times.AtLeastOnce());
            Assert.IsNotNull(result);
            var duplicatedTargetDates = result.Select(x => x.TimesheetDate);

            // Ensure whether efforts duplicated to all target dated.
            Assert.IsTrue(duplicatedTargetDates.All(duplicatedTargetDate => targetDates.Contains(duplicatedTargetDate)));
        }

        /// <summary>
        /// Tests whether previous month dates received in case if timesheet is not frozen.
        /// </summary>
        [TestMethod]
        public void GetNotYetFrozenTimesheetDates_PreviousMonthDatesProvided_PreviousMonthDatesReceived()
        {
            var previousMonthDates = new List<DateTime>
            {
                new DateTime(2020, 12, 02),
            };

            var notYetFrozenTimesheetDates = this.timesheetHelper.GetNotYetFrozenTimesheetDates(previousMonthDates, new DateTime(2021, 01, 02));

            Assert.IsNotNull(notYetFrozenTimesheetDates);

            // Ensures to receive all previous month dates.
            Assert.IsTrue(notYetFrozenTimesheetDates.All(notYetFrozenTimesheetDate => previousMonthDates.Contains(notYetFrozenTimesheetDate)));
        }

        /// <summary>
        /// Tests whether single day timesheet get returned.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetTimesheetsAsync_ActiveProjectsExistsAndAssignedToUser_ReturnsTimesheetOfADay()
        {
            var projectRepository = new Mock<IProjectRepository>();

            projectRepository
                .Setup(x => x.GetProjectsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.projects.AsEnumerable()));

            var timesheetRepository = new Mock<ITimesheetRepository>();

            timesheetRepository
                .Setup(x => x.GetTimesheetsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.timesheets));

            this.repositoryAccessors.Setup(x => x.ProjectRepository).Returns(projectRepository.Object);
            this.repositoryAccessors.Setup(x => x.TimesheetRepository).Returns(timesheetRepository.Object);

            var result = await this.timesheetHelper.GetTimesheetsAsync(new DateTime(2021, 01, 02), new DateTime(2021, 01, 02), Guid.Parse("e9be1d47-2707-4dfc-b2a9-e62648c3a04e"));

            Assert.IsNotNull(result);

            // The count ensures that the single day timesheet get received.
            Assert.IsTrue(result.Count() == 1);
        }

        /// <summary>
        /// Tests whether user Timesheets are not available.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task GetTimesheetsAsync_NoActiveProjectAssignedToUser_ReturnsZeroUserTimesheets()
        {
            var projectRepository = new Mock<IProjectRepository>();

            projectRepository
                .Setup(x => x.GetProjectsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.projects.AsEnumerable()));

            var timesheetRepository = new Mock<ITimesheetRepository>();

            timesheetRepository
                .Setup(x => x.GetTimesheetsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(this.timesheets));

            this.repositoryAccessors.Setup(x => x.ProjectRepository).Returns(projectRepository.Object);
            this.repositoryAccessors.Setup(x => x.TimesheetRepository).Returns(timesheetRepository.Object);

            var result = await this.timesheetHelper.GetTimesheetsAsync(new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), Guid.Parse("e9be1d47-2707-4dfc-b2a9-e62648c3a04e"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 0);
        }

        /// <summary>
        /// Test whether true is return with valid parameters while approving or rejecting timesheets.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task ApproveOrRejectTimesheets_WithValidParams_ShouldReturnTrue()
        {
            // ARRANGE
            this.repositoryAccessors.Setup(repositoryAccessor => repositoryAccessor.TimesheetRepository).Returns(() => this.timesheetRepository.Object);
            this.repositoryAccessors.Setup(x => x.Context).Returns(FakeTimesheetContext.GetFakeTimesheetContext());
            this.repositoryAccessors
                .Setup(accessor => accessor.SaveChangesAsync())
                .Returns(Task.FromResult(TestData.SubmittedTimesheets.Count));

            var managerId = Guid.NewGuid();

            // ACT
            var result = await this.timesheetHelper.ApproveOrRejectTimesheetsAsync(TestData.SubmittedTimesheets, TestData.RequestApprovalDTOs, TimesheetStatus.Approved);

            // ASSERT
            Assert.IsTrue(result);
            this.timesheetRepository.Verify(timesheetRepo => timesheetRepo.Update(It.IsAny<List<TimesheetEntity>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Test whether false status is return when failure at database while approving or rejecting timesheets.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        [TestMethod]
        public async Task ApproveOrRejectTimesheets_WhenFailureAtDatabase_ShouldRetunFalse()
        {
            // ARRANGE
            this.repositoryAccessors.Setup(repositoryAccessor => repositoryAccessor.TimesheetRepository).Returns(() => this.timesheetRepository.Object);
            this.repositoryAccessors.Setup(x => x.Context).Returns(FakeTimesheetContext.GetFakeTimesheetContext());
            this.repositoryAccessors
                .Setup(accessor => accessor.SaveChangesAsync())
                .Returns(Task.FromResult(0));

            var managerId = Guid.NewGuid();

            // ACT
            var result = await this.timesheetHelper.ApproveOrRejectTimesheetsAsync(TestData.SubmittedTimesheets, TestData.RequestApprovalDTOs, TimesheetStatus.Approved);

            // ASSERT
            Assert.IsFalse(result);
            this.timesheetRepository.Verify(timesheetRepo => timesheetRepo.Update(It.IsAny<List<TimesheetEntity>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Test whether valid requests are return with valid parameter while fetching requests.
        /// </summary>
        [TestMethod]
        public void GetTimesheetsByStatus_WithValidParams_ShouldReturnValidData()
        {
            // ARRANGE
            var expectedSubmittedRequestDTO = new List<SubmittedRequestDTO>
            {
                new SubmittedRequestDTO
                {
                    Status = (int)TimesheetStatus.Submitted,
                    UserId = Guid.Parse("1a1a285f-7b97-45a8-82c3-58562b69a1ce"),
                    TimesheetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25),
                    ProjectTitles = new List<string>
                    {
                        "Project 1",
                    },
                    SubmittedTimesheetIds = new List<Guid>
                    {
                        Guid.Parse("0a0a285f-7b97-45a8-82c3-58562b69a1ce"),
                    },
                    TotalHours = 10,
                },
            };
            this.repositoryAccessors.Setup(repositoryAccessor => repositoryAccessor.TimesheetRepository).Returns(() => this.timesheetRepository.Object);
            this.timesheetRepository
                .Setup(timeRepo => timeRepo.GetTimesheetOfUsersByStatus(It.IsAny<List<Guid>>(), It.IsAny<TimesheetStatus>()))
                .Returns(TestData.SubmittedTimesheets.AsEnumerable().GroupBy(x => x.UserId).ToDictionary(x => x.Key, x => x.AsEnumerable()));

            var userId = Guid.NewGuid();

            // ACT
            var result = this.timesheetHelper.GetTimesheetsByStatus(userId, TimesheetStatus.Approved).ToList();

            // ASSERT
            Assert.AreEqual(expectedSubmittedRequestDTO.Count, result.Count);
            this.timesheetRepository
                .Verify(timesheetRepo => timesheetRepo.GetTimesheetOfUsersByStatus(It.IsAny<List<Guid>>(), It.IsAny<TimesheetStatus>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Test whether timesheets is return with valid Ids while fetching submitted timesheets.
        /// </summary>
        [TestMethod]
        public void GetSubmittedTimesheetsByIds_WithValidIds_ShouldRetunValidData()
        {
            // ARRANGE
            this.repositoryAccessors.Setup(repositoryAccessor => repositoryAccessor.TimesheetRepository).Returns(() => this.timesheetRepository.Object);
            this.timesheetRepository
                .Setup(repository => repository.GetSubmittedTimesheetByIds(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()))
                .Returns(TestData.SubmittedTimesheets);

            var managerId = Guid.NewGuid();
            var timesheetIds = TestData.SubmittedTimesheets.Select(timesheet => timesheet.Id);

            // ACT
            var result = this.timesheetHelper.GetSubmittedTimesheetsByIds(managerId, timesheetIds);

            // ASSERT
            Assert.AreEqual(timesheetIds.Count(), result.Count());
            Assert.AreEqual(timesheetIds.First(), result.First().Id);
        }

        /// <summary>
        /// Test whether null is return with invalid Ids while fetching submitted timesheets.
        /// </summary>
        [TestMethod]
        public void GetSubmittedTimesheetsByIds_WithInvalidIds_ShouldRetunNull()
        {
            // ARRANGE
            this.repositoryAccessors.Setup(repositoryAccessor => repositoryAccessor.TimesheetRepository).Returns(() => this.timesheetRepository.Object);
            this.timesheetRepository
                .Setup(repository => repository.GetSubmittedTimesheetByIds(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()))
                .Returns(TestData.SubmittedTimesheets);

            var managerId = Guid.NewGuid();
            var timesheetIds = new List<Guid> { Guid.NewGuid() };

            // ACT
            var result = this.timesheetHelper.GetSubmittedTimesheetsByIds(managerId, timesheetIds);

            // ASSERT
            Assert.IsNull(result);
        }
    }
}
