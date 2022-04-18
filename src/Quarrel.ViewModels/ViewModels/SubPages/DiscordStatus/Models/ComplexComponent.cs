// Quarrel © 2022

using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.DiscordStatus.Models
{
    public class ComplexComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexComponent"/> class.
        /// </summary>
        public ComplexComponent(
            string name,
            string status,
            List<SimpleComponent> items)
        {
            Name = name;
            Status = status;
            Items = items;
        }

        public string Name { get; set; }

        public string Status { get; set; }

        public List<SimpleComponent> Items { get; set; }
    }
}
