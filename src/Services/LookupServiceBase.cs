using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Base class for lookup services that maintain an Index collection of type T.
    /// Provides GetIndex() to retrieve the full collection and abstract GetDefault()
    /// for subclass-specific default resolution.
    /// </summary>
    /// <typeparam name="T">The model type stored in the lookup index.</typeparam>
    public abstract class LookupServiceBase<T> : ILookupService<T>
    {
        protected readonly IList<T> Index;

        public LookupServiceBase()
        {
            Index = new ObservableCollection<T>();
        }

        public IList<T> GetIndex()
        {
            return Index;
        }

        public abstract T GetDefault();
    }
}
