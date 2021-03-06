// <copyright file="ConversationRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.Common.Repositories
{
    using Microsoft.Teams.Apps.Timesheet.Common.Models;

    /// <summary>
    /// This class manages all database operations related to user conversation entity.
    /// </summary>
    public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConversationRepository"/> class.
        /// </summary>
        /// <param name="context">The timesheet context.</param>
        public ConversationRepository(TimesheetContext context)
            : base(context)
        {
        }
    }
}