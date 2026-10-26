namespace Savaged.BlackNotepad.Models
{
    /// <summary>
    /// Represents a selectable font color item in the font color palette.
    /// </summary>
    public class FontColourModel :
        SelectionModelBase<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontColourModel"/> class.
        /// </summary>
        /// <param name="name">The color name or hex representation used as the resource key.</param>
        /// <param name="displayName">The human-readable display name shown in menus.</param>
        public FontColourModel(
            string name = "White", 
            string displayName = "White")
        : base(name, displayName) { }
    }
}
