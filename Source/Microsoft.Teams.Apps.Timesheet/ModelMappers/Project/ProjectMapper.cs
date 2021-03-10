// <copyright file="ProjectMapper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.ModelMappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Teams.Apps.Timesheet.Common.Extensions;
    using Microsoft.Teams.Apps.Timesheet.Common.Models;
    using Microsoft.Teams.Apps.Timesheet.Models;

    /// <summary>
    /// A model class that contains methods related to project model mappings.
    /// </summary>
    public class ProjectMapper : IProjectMapper
    {
        /// <summary>
        /// Gets project model to be inserted in database.
        /// </summary>
        /// <param name="projectViewModel">Project entity view model.</param>
        /// <param name="userObjectId">Azure Active Directory Id of logged-in user.</param>
        /// <returns>Returns a project entity model.</returns>
        public Project MapForCreateModel(ProjectDTO projectViewModel, Guid userObjectId)
        {
            projectViewModel = projectViewModel ?? throw new ArgumentNullException(nameof(projectViewModel));

            return new Project
            {
                BillableHours = projectViewModel.BillableHours,
                ClientName = projectViewModel.ClientName,
                EndDate = projectViewModel.EndDate,
                NonBillableHours = projectViewModel.NonBillableHours,
                StartDate = projectViewModel.StartDate,
                CreatedBy = userObjectId,
                CreatedOn = DateTime.UtcNow,
                Title = projectViewModel.Title,
                Members = projectViewModel.Members.IsNullOrEmpty() ? new List<Member>() :
                    projectViewModel.Members.Select(member => new Member
                    {
                        IsBillable = member.IsBillable,
                        UserId = member.UserId,
                        IsRemoved = false,
                    }).ToList(),
                Tasks = projectViewModel.Tasks.IsNullOrEmpty() ? new List<TaskEntity>() :
                    projectViewModel.Tasks.Select(task => new TaskEntity
                    {
                        Title = task.Title,
                        IsRemoved = false,
                    }).ToList(),
            };
        }

        /// <summary>
        /// Gets project model to be updated in database.
        /// </summary>
        /// <param name="projectViewModel">Project entity view model.</param>
        /// <param name="projectModel">Project entity model.</param>
        /// <returns>Returns a project entity model.</returns>
        public Project MapForUpdateModel(ProjectUpdateDTO projectViewModel, Project projectModel)
        {
            projectViewModel = projectViewModel ?? throw new ArgumentNullException(nameof(projectViewModel));
            projectModel = projectModel ?? throw new ArgumentNullException(nameof(projectModel));

            projectModel.BillableHours = projectViewModel.BillableHours;
            projectModel.ClientName = projectViewModel.ClientName;
            projectModel.EndDate = projectViewModel.EndDate;
            projectModel.NonBillableHours = projectViewModel.NonBillableHours;
            projectModel.StartDate = projectViewModel.StartDate;
            projectModel.Title = projectViewModel.Title;

            return projectModel;
        }

        /// <summary>
        /// Gets project view model to be sent as API response.
        /// </summary>
        /// <param name="projectModel">Project entity model.</param>
        /// <returns>Returns a project view entity model.</returns>
        public ProjectDTO MapForViewModel(Project projectModel)
        {
            projectModel = projectModel ?? throw new ArgumentNullException(nameof(projectModel));

            return new ProjectDTO
            {
                Title = projectModel.Title,
                BillableHours = projectModel.BillableHours,
                ClientName = projectModel.ClientName,
                EndDate = projectModel.EndDate,
                Id = projectModel.Id,
                NonBillableHours = projectModel.NonBillableHours,
                StartDate = projectModel.StartDate,
                Tasks = projectModel.Tasks.IsNullOrEmpty() ? new List<TaskDTO>() :
                    projectModel.Tasks.Select(task => new TaskDTO
                    {
                        Id = task.Id,
                        ProjectId = task.ProjectId,
                        Title = task.Title,
                    }).ToList(),
                Members = projectModel.Members.IsNullOrEmpty() ? new List<MemberDTO>() :
                    projectModel.Members.Select(member => new MemberDTO
                    {
                        Id = member.Id,
                        IsBillable = member.IsBillable,
                        ProjectId = member.ProjectId,
                        UserId = member.UserId,
                    }).ToList(),
            };
        }

        /// <summary>
        /// Gets project utilization view model to be sent as API response.
        /// </summary>
        /// <param name="project">The project entity model.</param>
        /// <param name="timesheets">Collection of timesheet entity model.</param>
        /// <returns>Returns a project utilization view entity model.</returns>
        public ProjectUtilizationDTO MapForProjectUtilizationViewModel(Project project, IEnumerable<TimesheetEntity> timesheets)
        {
            project = project ?? throw new ArgumentNullException(nameof(project));

            var totalHours = project.BillableHours + project.NonBillableHours;
            var projectUtilization = new ProjectUtilizationDTO
            {
                Id = project.Id,
                Title = project.Title,
                BillableHours = project.BillableHours,
                NonBillableHours = project.NonBillableHours,
                NotUtilizedHours = totalHours - timesheets.Sum(timesheet => timesheet.Hours),
            };

            return projectUtilization;
        }
    }
}