// Adam Dernis © 2022

using Discord;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Services.Discord;

namespace Quarrel.Models.Bindables.Abstract
{
    public abstract class BindableDiscordItemBase<T> : ObservableObject
        where T : IEntity<ulong>
    {
        private readonly IDiscordService _discordService;

        private T _model;

        protected BindableDiscordItemBase(IDiscordService discordService, T model)
        {
            _discordService = discordService;

            _model = model;
        }

        public T Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        public ulong Id => _model.Id;

        protected IDiscordService DiscordService => _discordService;

        public abstract void UpdateFromServiceAsync();

        protected virtual void UpdateFromModel(T model)
        {
            Guard.IsEqualTo(_model.Id, model.Id, nameof(Model.Id));

            Model = model;
        }
    }
}
