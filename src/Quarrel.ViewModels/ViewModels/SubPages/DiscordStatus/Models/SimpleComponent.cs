// Quarrel © 2022

namespace Quarrel.ViewModels.SubPages.DiscordStatus.Models
{
    public class SimpleComponent
    {
        public SimpleComponent(
            string name,
            string description,
            string status)
        {
            Name = name;
            Description = description;
            Status = status;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
    }
}
