// Quarrel © 2022

namespace Quarrel.ViewModels.SubPages.DiscordStatus.Models
{
    /// <summary>
    /// A component of the discord api.
    /// </summary>
    public class BindableComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableComponent"/> class.
        /// </summary>
        public BindableComponent(
            string name,
            string status,
            string? description)
        {
            Name = name;
            Description = description;
            Status = status;
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// Gets the status of the component.
        /// </summary>
        public string Status { get; }
    }
}
