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
using Savaged.BlackNotepad.Services;
using System.Linq;

namespace BlackNotepad.Test.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="FontColourLookupService"/> verifying palette expansion, defaults, and counterpart mappings.
    /// </summary>
    [TestClass]
    public class FontColourLookupServiceTests
    {
        private FontColourLookupService _service;

        /// <summary>
        /// Initializes test fixtures.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _service = new FontColourLookupService();
        }

        /// <summary>
        /// Verifies that the palette contains all 12 expected colors including Black and White.
        /// </summary>
        [TestMethod]
        public void GetIndex_ContainsExpectedColors_IncludingBlackAndWhite()
        {
            var index = _service.GetIndex();

            Assert.IsNotNull(index);
            Assert.AreEqual(12, index.Count);
            Assert.IsTrue(index.Any(c => c.Key == "Black"));
            Assert.IsTrue(index.Any(c => c.Key == "White"));
            Assert.IsTrue(index.Any(c => c.Key == "LightGreen"));
            Assert.IsTrue(index.Any(c => c.Key == "DarkGreen"));
            Assert.IsTrue(index.Any(c => c.Key == "Cyan"));
            Assert.IsTrue(index.Any(c => c.Key == "Navy"));
            Assert.IsTrue(index.Any(c => c.Key == "Gold"));
            Assert.IsTrue(index.Any(c => c.Key == "SaddleBrown"));
            Assert.IsTrue(index.Any(c => c.Key == "LightCoral"));
            Assert.IsTrue(index.Any(c => c.Key == "Crimson"));
            Assert.IsTrue(index.Any(c => c.Key == "LightGray"));
            Assert.IsTrue(index.Any(c => c.Key == "DarkSlateGray"));
        }

        /// <summary>
        /// Verifies default font color for dark theme is White.
        /// </summary>
        [TestMethod]
        public void GetDefault_DarkTheme_ReturnsWhite()
        {
            var def = _service.GetDefault();
            Assert.IsNotNull(def);
            Assert.AreEqual("White", def.Key);

            var darkDef = _service.GetDefault(ThemeMode.Dark);
            Assert.AreEqual("White", darkDef.Key);
        }

        /// <summary>
        /// Verifies default font color for light theme is Black.
        /// </summary>
        [TestMethod]
        public void GetDefault_LightTheme_ReturnsBlack()
        {
            var lightDef = _service.GetDefault(ThemeMode.Light);
            Assert.IsNotNull(lightDef);
            Assert.AreEqual("Black", lightDef.Key);
        }

        /// <summary>
        /// Verifies symmetrical mapping of dark-theme colors to high-contrast light counterparts.
        /// </summary>
        [TestMethod]
        public void GetCounterpart_DarkToLight_MapsSymmetrically()
        {
            Assert.AreEqual("Black", _service.GetCounterpart(new FontColourModel("White"), ThemeMode.Light).Key);
            Assert.AreEqual("DarkGreen", _service.GetCounterpart(new FontColourModel("LightGreen"), ThemeMode.Light).Key);
            Assert.AreEqual("Navy", _service.GetCounterpart(new FontColourModel("Cyan"), ThemeMode.Light).Key);
            Assert.AreEqual("SaddleBrown", _service.GetCounterpart(new FontColourModel("Gold"), ThemeMode.Light).Key);
            Assert.AreEqual("Crimson", _service.GetCounterpart(new FontColourModel("LightCoral"), ThemeMode.Light).Key);
            Assert.AreEqual("DarkSlateGray", _service.GetCounterpart(new FontColourModel("LightGray"), ThemeMode.Light).Key);
        }

        /// <summary>
        /// Verifies symmetrical mapping of light-theme colors to high-contrast dark counterparts.
        /// </summary>
        [TestMethod]
        public void GetCounterpart_LightToDark_MapsSymmetrically()
        {
            Assert.AreEqual("White", _service.GetCounterpart(new FontColourModel("Black"), ThemeMode.Dark).Key);
            Assert.AreEqual("LightGreen", _service.GetCounterpart(new FontColourModel("DarkGreen"), ThemeMode.Dark).Key);
            Assert.AreEqual("Cyan", _service.GetCounterpart(new FontColourModel("Navy"), ThemeMode.Dark).Key);
            Assert.AreEqual("Gold", _service.GetCounterpart(new FontColourModel("SaddleBrown"), ThemeMode.Dark).Key);
            Assert.AreEqual("LightCoral", _service.GetCounterpart(new FontColourModel("Crimson"), ThemeMode.Dark).Key);
            Assert.AreEqual("LightGray", _service.GetCounterpart(new FontColourModel("DarkSlateGray"), ThemeMode.Dark).Key);
        }

        /// <summary>
        /// Verifies that colors already compatible with the target theme are retained without inversion.
        /// </summary>
        [TestMethod]
        public void GetCounterpart_AlreadyCompatibleWithTarget_PreservesColor()
        {
            Assert.AreEqual("Black", _service.GetCounterpart(new FontColourModel("Black"), ThemeMode.Light).Key);
            Assert.AreEqual("DarkGreen", _service.GetCounterpart(new FontColourModel("DarkGreen"), ThemeMode.Light).Key);

            Assert.AreEqual("White", _service.GetCounterpart(new FontColourModel("White"), ThemeMode.Dark).Key);
            Assert.AreEqual("LightGreen", _service.GetCounterpart(new FontColourModel("LightGreen"), ThemeMode.Dark).Key);
        }

        /// <summary>
        /// Verifies that null, empty, or unknown colors fall back gracefully to theme defaults.
        /// </summary>
        [TestMethod]
        public void GetCounterpart_NullOrUnknown_ReturnsSafeDefault()
        {
            Assert.AreEqual("Black", _service.GetCounterpart(null, ThemeMode.Light).Key);
            Assert.AreEqual("White", _service.GetCounterpart(null, ThemeMode.Dark).Key);

            Assert.AreEqual("Black", _service.GetCounterpart(new FontColourModel("NonExistentCustomColor"), ThemeMode.Light).Key);
            Assert.AreEqual("White", _service.GetCounterpart(new FontColourModel("NonExistentCustomColor"), ThemeMode.Dark).Key);
        }
    }
}
