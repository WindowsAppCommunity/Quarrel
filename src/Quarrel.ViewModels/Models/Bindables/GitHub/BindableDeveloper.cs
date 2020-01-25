using GitHubAPI.API;
using GitHubAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Models.Bindables.GitHub
{
    public class BindableDeveloper : BindableModelBase<User>
    {
        #region Constructor

        public BindableDeveloper(User user, Contributor contributor) : base(user)
        {
            Contributor = contributor;
        }

        #endregion

        #region Properties

        public Contributor Contributor { get; set; }

        #endregion
    }
}
