// <copyright file="ProjectUtilizationDTO.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.Models
{
    using System;

    /// <summary>
    /// Holds the details of a project utilization entity.
    /// </summary>
    public class ProjectUtilizationDTO
    {
        /// <summary>
        /// Gets or sets Id of project.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets title of project.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets billable hours of project.
        /// </summary>
        public int BillableHours { get; set; }

        /// <summary>
        /// Gets or sets non billable hours of project.
        /// </summary>
        public int NonBillableHours { get; set; }

        /// <summary>
        /// Gets or sets not utilized hours of project.
        /// </summary>
        public int NotUtilizedHours { get; set; }
    }
}
