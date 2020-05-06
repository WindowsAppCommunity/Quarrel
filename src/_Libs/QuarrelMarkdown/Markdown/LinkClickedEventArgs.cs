// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Channels;
using System;

namespace Quarrel.Controls.Markdown
{
    /// <summary>
    /// Arguments for the OnLinkClicked event which is fired then the user presses a link.
    /// </summary>
    public class LinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkClickedEventArgs"/> class.
        /// </summary>
        /// <param name="link">Raw link url or raw mention.</param>
        internal LinkClickedEventArgs(string link)
        {
            Link = link;
        }

        /// <summary>
        /// Gets the link that was tapped.
        /// </summary>
        public string Link { get; }

        /// <summary>
        /// Gets or sets the <see cref="User"/> mention clicked.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BindableChannel"/> mention clicked.
        /// </summary>
        public BindableChannel Channel { get; set; }
    }
}
