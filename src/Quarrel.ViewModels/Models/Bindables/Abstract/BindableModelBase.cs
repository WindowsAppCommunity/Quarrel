// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight;
using JetBrains.Annotations;

namespace Quarrel.ViewModels.Models.Bindables.Abstract
{
    /// <summary>
    /// A wrapper for ViewModelBase given a guarentedd Model context.
    /// </summary>
    /// <typeparam name="T">The wrapped type.</typeparam>
    public class BindableModelBase<T> : ViewModelBase
        where T : class
    {
        private T _model;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableModelBase{T}"/> class.
        /// </summary>
        /// <param name="model">The orginal model object.</param>
        protected BindableModelBase([NotNull] T model) => Model = model;

        /// <summary>
        /// Gets or sets the wrapped model associated with the current instance.
        /// </summary>
        [NotNull]
        public T Model
        {
            get => _model;
            set => Set(ref _model, value);
        }
    }
}
