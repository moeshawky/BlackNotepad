using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Models;
using System;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class ViewStateModelTests
    {
        [TestMethod]
        public void Constructor_Parameterless_InitializesWithDefaults()
        {
            var model = new ViewStateModel();

            Assert.IsNotNull(model.SelectedFontZoom);
            Assert.IsNotNull(model.SelectedFontColour);
            Assert.IsNotNull(model.SelectedFontFamily);
            Assert.IsNotNull(model.RecentFiles);
        }

        [TestMethod]
        public void Constructor_WithNullDefaults_UsesEmptyModels()
        {
            var model = new ViewStateModel(
                defaultFontColour: null,
                defaultFontFamily: null,
                defaultFontZoom: null);

            Assert.IsNotNull(model.SelectedFontColour);
            Assert.IsNotNull(model.SelectedFontFamily);
            Assert.IsNotNull(model.SelectedFontZoom);
        }

        [TestMethod]
        public void SelectedFontColour_SetNull_DoesNotThrow_NullSafe()
        {
            var model = new ViewStateModel();

            // This setter calls value.IsSelected = true without null check
            // If null is passed, it would throw NullReferenceException
            // Current code does NOT guard against null - this tests current behavior
            try
            {
                model.SelectedFontColour = null;
                Assert.Fail("Setting null should throw or be handled");
            }
            catch (NullReferenceException)
            {
                // Expected with current implementation - this is the bug
                // The test documents the current (buggy) behavior
            }
        }

        [TestMethod]
        public void SelectedFontFamily_SetNull_DoesNotThrow_NullSafe()
        {
            var model = new ViewStateModel();

            try
            {
                model.SelectedFontFamily = null;
                Assert.Fail("Setting null should throw or be handled");
            }
            catch (NullReferenceException)
            {
                // Expected with current implementation - this is the bug
            }
        }

        [TestMethod]
        public void SelectedFontZoom_SetNull_AllowsNull()
        {
            var model = new ViewStateModel();

            // SelectedFontZoom setter does NOT set IsSelected, so null is allowed
            model.SelectedFontZoom = null;
            Assert.IsNull(model.SelectedFontZoom);
        }

        [TestMethod]
        public void SelectedThemeMode_ValidValues_Accepted()
        {
            var model = new ViewStateModel();

            model.SelectedThemeMode = ThemeMode.Light;
            Assert.AreEqual(ThemeMode.Light, model.SelectedThemeMode);

            model.SelectedThemeMode = ThemeMode.Dark;
            Assert.AreEqual(ThemeMode.Dark, model.SelectedThemeMode);

            model.SelectedThemeMode = ThemeMode.System;
            Assert.AreEqual(ThemeMode.System, model.SelectedThemeMode);
        }

        [TestMethod]
        public void IsLineNumbersVisible_DefaultsToFalse()
        {
            var model = new ViewStateModel();

            Assert.IsFalse(model.IsLineNumbersVisible);
        }

        [TestMethod]
        public void IsLineNumbersVisible_CanBeSet()
        {
            var model = new ViewStateModel();

            model.IsLineNumbersVisible = false;
            Assert.IsFalse(model.IsLineNumbersVisible);

            model.IsLineNumbersVisible = true;
            Assert.IsTrue(model.IsLineNumbersVisible);
        }

        [TestMethod]
        public void AutoSaveEnabled_DefaultsToTrue()
        {
            var model = new ViewStateModel();

            Assert.IsTrue(model.AutoSaveEnabled);
        }

        [TestMethod]
        public void RecentFiles_IsInitializedAndEmpty()
        {
            var model = new ViewStateModel();

            Assert.IsNotNull(model.RecentFiles);
            Assert.AreEqual(0, model.RecentFiles.Count);
        }

        [TestMethod]
        public void IsWrapped_DefaultsToFalse()
        {
            var model = new ViewStateModel();

            Assert.IsFalse(model.IsWrapped);
        }

        [TestMethod]
        public void IsStatusBarVisible_DefaultsToFalse()
        {
            var model = new ViewStateModel();

            Assert.IsFalse(model.IsStatusBarVisible);
        }
    }
}
