// Quarrel © 2022

using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.DiscordStatus.Models
{
    /// <summary>
    /// An incident including its affected components.
    /// </summary>
    public class BindableIncident
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableIncident"/> class.
        /// </summary>
        public BindableIncident(
            string name,
            string status,
            List<BindableComponent> items)
        {
            Name = name;
            Status = status;
            Items = items;
        }

        /// <summary>
        /// Gets the name of the incident.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the status of the incident.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Gets a list of the affected components of the incident.
        /// </summary>
        public List<BindableComponent> Items { get; }
    }
}
