// Copyright (C) 2026 Moe S. / BlackNotepad Contributors
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Models;

namespace BlackNotepad.Test.ViewModelTests
{
    /// <summary>
    /// ViewModel tests verifying tactful, dynamic font color switching across theme transitions.
    /// </summary>
    [TestClass]
    public class ThemeModeFontSwitchingTests : TestBase
    {
        /// <summary>
        /// Verifies that switching from Dark mode with default White text to Light mode adapts font to Black.
        /// </summary>
        [TestMethod]
        public void ThemeMode_SwitchFromDarkToLight_AdaptsFontColourToBlack()
        {
            Assert.AreEqual(ThemeMode.Dark, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("White", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Light");

            Assert.AreEqual(ThemeMode.Light, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("Black", MainVm.ViewState.SelectedFontColour.Key);
        }

        /// <summary>
        /// Verifies that switching from Light mode with Black text back to Dark mode adapts font to White.
        /// </summary>
        [TestMethod]
        public void ThemeMode_SwitchFromLightToDark_AdaptsFontColourToWhite()
        {
            MainVm.ThemeModeCmd.Execute("Light");
            Assert.AreEqual(ThemeMode.Light, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("Black", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Dark");

            Assert.AreEqual(ThemeMode.Dark, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("White", MainVm.ViewState.SelectedFontColour.Key);
        }

        /// <summary>
        /// Verifies that custom accent colors preserve their semantic color family across round trips.
        /// </summary>
        [TestMethod]
        public void ThemeMode_SwitchWithAccentColors_PreservesColorFamilyAcrossRoundTrip()
        {
            MainVm.FontColourCmd.Execute(new FontColourModel("LightGreen", "Light Green"));
            Assert.AreEqual("LightGreen", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Light");
            Assert.AreEqual(ThemeMode.Light, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("DarkGreen", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Dark");
            Assert.AreEqual(ThemeMode.Dark, MainVm.ViewState.SelectedThemeMode);
            Assert.AreEqual("LightGreen", MainVm.ViewState.SelectedFontColour.Key);
        }

        /// <summary>
        /// Verifies Cyan in Dark mode maps to Navy in Light mode and back to Cyan.
        /// </summary>
        [TestMethod]
        public void ThemeMode_SwitchToCyanInDark_AdaptsToNavyInLight_AndBack()
        {
            MainVm.FontColourCmd.Execute(new FontColourModel("Cyan", "Cyan"));
            Assert.AreEqual("Cyan", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Light");
            Assert.AreEqual("Navy", MainVm.ViewState.SelectedFontColour.Key);

            MainVm.ThemeModeCmd.Execute("Dark");
            Assert.AreEqual("Cyan", MainVm.ViewState.SelectedFontColour.Key);
        }
    }
}
