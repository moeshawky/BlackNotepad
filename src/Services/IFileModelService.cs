using Savaged.BlackNotepad.Models;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Service for creating, loading, and saving document models.
    /// </summary>
    public interface IFileModelService
    {
        /// <summary>
        /// Creates a new empty FileModel with default values.
        /// </summary>
        /// <returns>A new FileModel instance.</returns>
        FileModel New();

        /// <summary>
        /// Loads file content from disk asynchronously.
        /// </summary>
        /// <param name="location">File path to read.</param>
        /// <returns>A FileModel with loaded content and metadata.</returns>
        Task<FileModel> LoadAsync(string location);

        /// <summary>
        /// Saves file content to disk asynchronously with default manual save reason.
        /// </summary>
        /// <param name="fileModel">The file model to save.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveAsync(FileModel fileModel);

        /// <summary>
        /// Saves file content to disk asynchronously with an explicit save reason.
        /// </summary>
        /// <param name="fileModel">The file model to save.</param>
        /// <param name="reason">The reason triggering the save operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveAsync(FileModel fileModel, SaveReason reason);
    }
}
