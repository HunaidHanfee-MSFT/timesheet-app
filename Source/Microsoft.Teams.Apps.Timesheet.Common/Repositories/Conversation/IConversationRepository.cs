// <copyright file="IConversationRepository.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// </copyright>

namespace Microsoft.Teams.Apps.Timesheet.Common.Repositories
{
    using Microsoft.Teams.Apps.Timesheet.Common.Models;

    /// <summary>
    /// Exposes methods which will be used to perform database operations on user conversation entity.
    /// </summary>
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
    }
}
