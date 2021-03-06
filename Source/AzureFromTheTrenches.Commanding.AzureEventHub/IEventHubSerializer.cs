﻿using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    /// <summary>
    /// Interface for an event hub seriailzer. The default serializer uses Json.
    /// </summary>
    public interface IEventHubSerializer
    {
        /// <summary>
        /// Return a serialized verison of an audit item
        /// </summary>
        /// <param name="auditItem">The audit item to serialize</param>
        /// <returns>Serialized audit item</returns>
        string Serialize(AuditItem auditItem);

        /// <summary>
        /// Returns a serialized version of a command
        /// </summary>
        /// <param name="command">The command to serialize</param>
        /// <returns>Serialized command</returns>
        string Serialize(ICommand command);
    }
}
