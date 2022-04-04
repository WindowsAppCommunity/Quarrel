// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Discord;

namespace Quarrel.Models.Bindables.Abstract
{
    public abstract class BindableUnqiueItemBase<T> : ObservableObject
        where T : ISnowflakeItem
    {
        private T _model;

        protected BindableUnqiueItemBase(T model)
        {
            _model = model;
        }

        public T Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public ulong Id => _model.Id;

        public abstract void UpdateFromServiceAsync();

        protected virtual void UpdateFromModel(T model)
        {
            Guard.IsEqualTo(_model.Id, model.Id, nameof(Model.Id));

            Model = model;
        }
    }
}
