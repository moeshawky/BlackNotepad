using Savaged.BlackNotepad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Service providing available font colors and theme-aware contrast counterpart mapping.
    /// </summary>
    public class FontColourLookupService :
        LookupServiceBase<FontColourModel>, IFontColourLookupService
    {
        private const string DefaultDark = "White";
        private const string DefaultLight = "Black";

        // Mappings between dark-theme colors (light text) and light-theme colors (dark text)
        private static readonly Dictionary<string, string> DarkToLightMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "White", "Black" },
                { "LightGreen", "DarkGreen" },
                { "Cyan", "Navy" },
                { "Gold", "SaddleBrown" },
                { "LightCoral", "Crimson" },
                { "LightGray", "DarkSlateGray" },
                // Aliases and graceful fallbacks
                { "Yellow", "SaddleBrown" },
                { "Lime", "DarkGreen" },
                { "Silver", "DarkSlateGray" },
                { "DeepSkyBlue", "Navy" }
            };

        private static readonly Dictionary<string, string> LightToDarkMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Black", "White" },
                { "DarkGreen", "LightGreen" },
                { "Navy", "Cyan" },
                { "SaddleBrown", "Gold" },
                { "Crimson", "LightCoral" },
                { "DarkSlateGray", "LightGray" },
                // Aliases and graceful fallbacks
                { "ForestGreen", "LightGreen" },
                { "DarkBlue", "Cyan" },
                { "Brown", "Gold" },
                { "DarkRed", "LightCoral" },
                { "DimGray", "LightGray" },
                { "Gray", "LightGray" }
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="FontColourLookupService"/> class.
        /// Populates available palette colors.
        /// </summary>
        public FontColourLookupService() : base()
        {
            Index.Add(new FontColourModel("Black", "Black"));
            Index.Add(new FontColourModel("White", "White"));
            Index.Add(new FontColourModel("LightGreen", "Light Green"));
            Index.Add(new FontColourModel("DarkGreen", "Dark Green"));
            Index.Add(new FontColourModel("Cyan", "Cyan"));
            Index.Add(new FontColourModel("Navy", "Navy"));
            Index.Add(new FontColourModel("Gold", "Amber"));
            Index.Add(new FontColourModel("SaddleBrown", "Dark Amber"));
            Index.Add(new FontColourModel("LightCoral", "Light Coral"));
            Index.Add(new FontColourModel("Crimson", "Crimson"));
            Index.Add(new FontColourModel("LightGray", "Light Gray"));
            Index.Add(new FontColourModel("DarkSlateGray", "Dark Slate Gray"));
        }

        /// <summary>
        /// Gets the default font color model.
        /// </summary>
        /// <returns>Default font color model (White).</returns>
        public override FontColourModel GetDefault()
        {
            return GetDefault(ThemeMode.Dark);
        }

        /// <summary>
        /// Gets the default font color model for the given theme mode.
        /// </summary>
        /// <param name="mode">The effective theme mode.</param>
        /// <returns>Black for Light mode; White for Dark/System mode.</returns>
        public FontColourModel GetDefault(ThemeMode mode)
        {
            string key = mode == ThemeMode.Light ? DefaultLight : DefaultDark;
            return Index.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase))
                ?? Index.First();
        }

        /// <summary>
        /// Gets the tactful counterpart font color model for the target theme.
        /// If the current color is already suitable for the target theme, it is retained.
        /// Otherwise, it maps to its reciprocal counterpart in the same color family.
        /// </summary>
        /// <param name="current">The current font color model.</param>
        /// <param name="targetMode">The target theme mode.</param>
        /// <returns>The adapted font color model.</returns>
        public FontColourModel GetCounterpart(FontColourModel current, ThemeMode targetMode)
        {
            if (current == null || string.IsNullOrWhiteSpace(current.Key))
            {
                return GetDefault(targetMode);
            }

            if (targetMode == ThemeMode.Light)
            {
                // If it's already one of the light-theme friendly colors, keep it
                if (LightToDarkMap.ContainsKey(current.Key))
                {
                    return FindInIndex(current.Key) ?? current;
                }

                // If it's a dark-theme color that lacks contrast on white, map it
                if (DarkToLightMap.TryGetValue(current.Key, out var lightTarget))
                {
                    return FindInIndex(lightTarget) ?? new FontColourModel(lightTarget, lightTarget);
                }

                // Unknown color: default to Black for readability
                return GetDefault(ThemeMode.Light);
            }
            else
            {
                // Dark mode (or System resolved to Dark):
                // If it's already one of the dark-theme friendly colors, keep it
                if (DarkToLightMap.ContainsKey(current.Key))
                {
                    return FindInIndex(current.Key) ?? current;
                }

                // If it's a light-theme color that lacks contrast on black, map it
                if (LightToDarkMap.TryGetValue(current.Key, out var darkTarget))
                {
                    return FindInIndex(darkTarget) ?? new FontColourModel(darkTarget, darkTarget);
                }

                // Unknown color: default to White for readability
                return GetDefault(ThemeMode.Dark);
            }
        }

        private FontColourModel FindInIndex(string key)
        {
            return Index.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase));
        }
    }
}
