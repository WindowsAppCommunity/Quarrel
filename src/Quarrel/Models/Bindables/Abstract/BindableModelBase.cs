// Special thanks to Sergio Pedri for the basis of this design

using GalaSoft.MvvmLight;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Models.Bindables.Abstract
{
    public class BindableModelBase<T> : ViewModelBase where T : class
    {
        public T _Model;

        /// <summary>
        /// Gets the wrapped model associated with the current instance
        /// </summary>
        [NotNull]
        public T Model
        {
            get => _Model;
            set => Set(ref _Model, value);
        }

        protected BindableModelBase([NotNull] T model) => Model = model;
    }
}
