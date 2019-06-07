using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;

namespace Quarrel.Classes
{
    public static class Extensions
    {
        public static T GetKey<T>(this OrderedDictionary dictionary, int index)
        {
            if (dictionary == null)
            {
                return default(T);
            }

            try
            {
                return (T)dictionary.Cast<DictionaryEntry>().ElementAt(index).Key;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }

    class LoadingStack
    {
        public class Loader
        {
            public Loader(string name, string status)
            {
                Name = name;
                Status = status;
            }
            public string Status { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// All components have finished loading
        /// </summary>
        public event EventHandler FinishedLoading;

        /// <summary>
        /// The status at the top of the stack has changed
        /// </summary>
        public event EventHandler<Loader> LoaderChanged;

        private OrderedDictionary loaders = new OrderedDictionary();

        /// <summary>
        /// Add a new loading indicator to the stack
        /// </summary>
        /// <param name="name"></param>
        /// <param name="status"></param>
        public void Loading(string name, string status)
        {
            if(!loaders.Contains(name))
                loaders.Add(name, new Loader(name, status));
            CheckLatest();
        }

        /// <summary>
        /// Remove a loading indicator from the stack
        /// </summary>
        /// <param name="name"></param>
        public void Loaded(string name)
        {
            if (loaders.Contains(name))
            {
                loaders.Remove(name);
            }
            if(loaders.Count == 0)
            {
                //No more loaders left, fire the FinishedLoading event.
                FinishedLoading?.Invoke(null, null);
            }
            else
            {
                CheckLatest();
            }
        }

        /// <summary>
        /// Remove all entries from the loading stack
        /// </summary>
        public void Clear()
        {
            loaders.Clear();
            FinishedLoading?.Invoke(null, null);
        }

        private string previousLoader = null;
        
        /// <summary>
        /// Invoke LoaderChanged
        /// </summary>
        private void CheckLatest()
        {
            if (loaders.Count == 0) return;
            var latest = ((Loader)loaders[loaders.Count-1]);
            if(latest.Name != previousLoader)
            {
                LoaderChanged?.Invoke(null, latest);
            }
        }
    }
}
