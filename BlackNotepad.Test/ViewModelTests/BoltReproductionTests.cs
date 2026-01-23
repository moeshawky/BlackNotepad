using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.ViewModels;
using System;

namespace BlackNotepad.Test.ViewModelTests
{
    [TestClass]
    public class BoltReproductionTests : TestBase
    {
        private IReplaceDialogViewModel _replace;

        [TestInitialize]
        public void SetUp()
        {
            _replace = MockDialogService.Object
                .GetDialogViewModel<IReplaceDialogViewModel>();
        }

        [TestMethod]
        public void ReplaceAll_MatchCase_ShouldNotReplaceMismatchingCase()
        {
            // Setup content with mixed case
            MainVm.SelectedItem.Content = "Foo foo FOO";
            const string sought = "Foo";
            const string replacement = "Bar";

            // Setup Replace Dialog
            MainVm.ReplaceCmd.Execute(null);
            _replace.TextSought = sought;
            _replace.ReplacementText = replacement;
            _replace.IsFindMatchCase = true; // Match Case = True

            // Execute Replace All
            _replace.RaiseReplaceAll();

            var text = MainVm.SelectedItem.Content;

            // Expectation: Only "Foo" is replaced. "foo" and "FOO" remain.
            // Legacy Bug: Replaces "foo" and "FOO" too because it uses IgnoreCase.
            Assert.AreEqual("Bar foo FOO", text, "Match Case=True should be case sensitive.");
        }

        [TestMethod]
        public void ReplaceAll_IgnoreCase_ShouldReplaceAllCases()
        {
            // Setup content
            MainVm.SelectedItem.Content = "Foo foo FOO";
            const string sought = "Foo";
            const string replacement = "Bar";

            // Setup Replace Dialog
            MainVm.ReplaceCmd.Execute(null);
            _replace.TextSought = sought;
            _replace.ReplacementText = replacement;
            _replace.IsFindMatchCase = false; // Match Case = False (Ignore Case)

            // Execute Replace All
            _replace.RaiseReplaceAll();

            var text = MainVm.SelectedItem.Content;

            // Expectation: All replaced.
            // Legacy Bug: Only replaces "Foo" because it uses Default (CaseSensitive).
            Assert.AreEqual("Bar Bar Bar", text, "Match Case=False should be case insensitive.");
        }

        [TestMethod]
        public void ReplaceAll_SpecialCharacters_ShouldNotCrash()
        {
            // Setup content with special regex characters
            MainVm.SelectedItem.Content = "Hello (World)";
            const string sought = "(World)";
            const string replacement = "Universe";

            // Setup Replace Dialog
            MainVm.ReplaceCmd.Execute(null);
            _replace.TextSought = sought;
            _replace.ReplacementText = replacement;
            _replace.IsFindMatchCase = true;

            // Execute Replace All
            // Legacy Bug: treated (World) as Regex group, or crashed on invalid regex.
            try
            {
                _replace.RaiseReplaceAll();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Crash detected: {ex.Message}");
            }

            var text = MainVm.SelectedItem.Content;

            // Expectation: Literal replacement of "(World)" -> "Universe"
            // Result: "Hello Universe"
            Assert.AreEqual("Hello Universe", text, "Should handle special characters literally.");
        }

        [TestMethod]
        public void ReplaceAll_InvalidRegex_ShouldNotCrash()
        {
             // Setup content
            MainVm.SelectedItem.Content = "Hello World";
            const string sought = "(Unclosed";
            const string replacement = "Fixed";

            // Setup Replace Dialog
            MainVm.ReplaceCmd.Execute(null);
            _replace.TextSought = sought;
            _replace.ReplacementText = replacement;
            _replace.IsFindMatchCase = true;

            // Execute Replace All
            // Legacy Bug: Crashes because it tries to parse "(Unclosed" as Regex.
            try
            {
                _replace.RaiseReplaceAll();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Crash detected on invalid regex input: {ex.Message}");
            }

            // Content should be unchanged as sought text is not found
            Assert.AreEqual("Hello World", MainVm.SelectedItem.Content);
        }
    }
}
