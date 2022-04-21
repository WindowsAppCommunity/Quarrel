// Quarrel © 2022

namespace Quarrel.RichPresence.Models
{
    /// <summary>
    /// The base class of an acitivity to display with rich presence.
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        public Activity(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the activity.
        /// </summary>
        public string Name { get; set; }
    }
}
