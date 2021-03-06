// <copyright file="UserController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Teams.Apps.Timesheet.Common.Extensions;
    using Microsoft.Teams.Apps.Timesheet.Models;
    using Microsoft.Teams.Apps.Timesheet.Services.MicrosoftGraph;

    /// <summary>
    /// User controller is responsible to expose API endpoints to fetch reportees and manager details.
    /// </summary>
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// Logs errors and information.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The instance of user Graph service to access logged-in user's reportees and manager.
        /// </summary>
        private readonly IUsersService userGraphService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The ILogger object which logs errors and information.</param>
        /// <param name="userGraphService">The instance of user Graph service to access logged-in user's reportees and manager.</param>
        /// <param name="telemetryClient">The Application Insights telemetry client.</param>
        public UserController(
            ILogger<UserController> logger,
            IUsersService userGraphService,
            TelemetryClient telemetryClient)
            : base(telemetryClient)
        {
            this.userGraphService = userGraphService;
            this.logger = logger;
        }

        /// <summary>
        /// Get direct reportees for logged-in user.
        /// </summary>
        /// <param name="search">Search text for querying over display name and email of user.</param>
        /// <returns>Returns list of users who report to logged-in user.</returns>
        [HttpGet("/api/me/reportees")]
        public async Task<IActionResult> GetMyReporteesAsync([FromQuery] string search)
        {
            this.RecordEvent("Get reportees- The HTTP GET call to get reportees has been initiated.", RequestType.Initiated);

            try
            {
                var reporteesResponse = await this.userGraphService.GetMyReporteesAsync(search);

                this.RecordEvent("Get reportees- The HTTP GET call to get reportees has succeeded.", RequestType.Succeeded);
                var reportees = reporteesResponse.Select(user => new ReporteeDTO
                {
                    DisplayName = user.DisplayName,
                    Id = Guid.Parse(user.Id),
                    UserPrincipalName = user.UserPrincipalName,
                });

                return this.Ok(reportees);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Get reportees- The HTTP GET call to get reportees has failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error occurred while fetching reportees.");
                throw;
            }
        }

        /// <summary>
        /// Get manager for logged-in user.
        /// </summary>
        /// <returns>Returns manager details.</returns>
        [HttpGet("/api/me/manager")]
        public async Task<IActionResult> GetManagerAsync()
        {
            this.RecordEvent("Get manager- The HTTP GET call to get manager has been initiated.", RequestType.Initiated);

            try
            {
                var managerDetails = await this.userGraphService.GetManagerAsync();

                this.RecordEvent("Get manager- The HTTP GET call to get manager has succeeded.", RequestType.Succeeded);

                return this.Ok(managerDetails);
            }
            catch (Exception ex)
            {
                this.RecordEvent("Get manager- The HTTP GET call to get manager has failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error occurred while fetching manager details.");
                throw;
            }
        }

        /// <summary>
        /// Get user profiles by user object Ids.
        /// </summary>
        /// <param name="userIds">List of user object Ids.</param>
        /// <returns>List of users profile.</returns>
        [HttpPost("/api/users")]
        public async Task<IActionResult> GetUsersProfileAsync([FromBody] IEnumerable<string> userIds)
        {
            this.RecordEvent("Get users profiles- The HTTP call to GET users profiles has been initiated.", RequestType.Initiated);

            if (userIds.IsNullOrEmpty())
            {
                this.RecordEvent("Get users profiles- The HTTP call to GET users profiles has been failed.", RequestType.Failed);
                this.logger.LogError("User Id list cannot be null or empty.");
                return this.BadRequest(new { message = "User Id list cannot be null or empty." });
            }

            try
            {
                var userProfiles = await this.userGraphService.GetUsersAsync(userIds);
                this.RecordEvent("Get users profiles- The HTTP call to GET users profiles has been succeeded.", RequestType.Succeeded);

                if (userProfiles != null)
                {
                    return this.Ok(userProfiles.Select(user => new UserDTO { DisplayName = user.Value.DisplayName, Id = user.Value.Id }));
                }

                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.RecordEvent("Get users profiles- The HTTP call to GET users profiles has been failed.", RequestType.Failed);
                this.logger.LogError(ex, "Error occurred while fetching users profiles.");
                throw;
            }
        }
    }
}
