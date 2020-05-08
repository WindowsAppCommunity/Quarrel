// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Models.Emojis
{
    /// <summary>
    /// Bindable ViewModel for Emoji DataContext.
    /// </summary>
    public class Emoji : ViewModelBase, IEquatable<Emoji>
    {
        private List<string> _names;
        private bool? _hasDiversity;
        private string _preview;

        /// <summary>
        /// Gets the Unique ID for the emoji.
        /// </summary>
        [JsonIgnore]
        public virtual string Id
        {
            get => Names?[0];
        }

        /// <summary>
        /// Gets or sets the split string by skin-tone suffix.
        /// </summary>
        [JsonProperty("names")]
        public List<string> Names
        {
            get => _names;
            set => Set(ref _names, value);
        }

        /// <summary>
        /// Gets or sets if the Emoji supports multiple skintones.
        /// </summary>
        [JsonProperty("hasDiversity")]
        public bool? HasDiversity
        {
            get => _hasDiversity;
            set => Set(ref _hasDiversity, value);
        }

        /// <summary>
        /// Gets a value indicating whether or not the emoji is from Discord (instead of Unicode).
        /// </summary>
        [JsonIgnore]
        public virtual bool CustomEmoji => false;

        /// <summary>
        /// Gets or sets the category the emoji is from.
        /// </summary>
        [JsonProperty("category")]
        public virtual string Category { get; set; }

        /// <summary>
        /// Gets or sets how the emoji is displayed in drafts.
        /// </summary>
        [JsonProperty("surrogates")]
        public string Preview
        {
            get => _preview;
            set => Set(ref _preview, value);
        }

        /// <summary>
        /// Gets colon format to paste.
        /// </summary>
        public virtual string Surrogate
        {
            get => Preview;
        }

        /// <summary>
        /// Checks if this is the same as other.
        /// </summary>
        /// <param name="other">Emoji to compare with.</param>
        /// <returns>True if this emoji is the same as other.</returns>
        public bool Equals(Emoji other)
        {
            return Id == other.Id;
        }
    }
}
