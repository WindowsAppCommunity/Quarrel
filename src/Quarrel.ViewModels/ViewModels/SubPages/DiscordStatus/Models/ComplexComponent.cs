// Quarrel © 2022

using System.Collections.Generic;

namespace Quarrel.ViewModels.SubPages.DiscordStatus.Models
{
    public class ComplexComponent
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Time { get; set; }

        public List<SimpleComponent> Items { get; set; }
    }
}
