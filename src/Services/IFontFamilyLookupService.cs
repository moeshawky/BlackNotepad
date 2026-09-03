using Savaged.BlackNotepad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    public interface IFontFamilyLookupService
    {
        FontFamilyModel GetDefault();
        IList<FontFamilyModel> GetIndex();
        /// <summary>
        /// Loads installed system fonts in the background. Safe to call once.
        /// </summary>
        Task LoadAsync();
    }
}