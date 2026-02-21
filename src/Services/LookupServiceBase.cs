using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Savaged.BlackNotepad.Services
{
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
