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
using Newtonsoft.Json;
using Savaged.BlackNotepad.Extensions;
using Savaged.BlackNotepad.Lookups;
using System;

namespace BlackNotepad.Test.UnitTests
{
    /// <summary>
    /// Unit tests verifying string extension methods including line index calculations and streaming JSON prettification.
    /// </summary>
    [TestClass]
    public class StringExTests
    {
        /// <summary>
        /// Verifies that PrettifyJson throws an ArgumentNullException when passed a null string reference.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PrettifyJson_NullInput_ThrowsArgumentNullException()
        {
            string nullString = null;
            nullString.PrettifyJson();
        }

        /// <summary>
        /// Verifies that PrettifyJson returns empty or whitespace strings unmodified.
        /// </summary>
        [TestMethod]
        public void PrettifyJson_EmptyOrWhitespace_ReturnsSameString()
        {
            Assert.AreEqual(string.Empty, string.Empty.PrettifyJson());
            Assert.AreEqual("   ", "   ".PrettifyJson());
            Assert.AreEqual("\t\r\n", "\t\r\n".PrettifyJson());
        }

        /// <summary>
        /// Verifies that PrettifyJson formats a single JSON object with standard indentation.
        /// </summary>
        [TestMethod]
        public void PrettifyJson_SingleJsonObject_FormatsWithIndentation()
        {
            string raw = "{\"name\":\"BlackNotepad\",\"version\":2,\"active\":true}";
            string formatted = raw.PrettifyJson();

            string expected = "{\r\n  \"name\": \"BlackNotepad\",\r\n  \"version\": 2,\r\n  \"active\": true\r\n}";
            Assert.AreEqual(expected.Replace("\r\n", Environment.NewLine), formatted.Replace("\r\n", Environment.NewLine));
        }

        /// <summary>
        /// Verifies that PrettifyJson formats a JSON array with standard indentation.
        /// </summary>
        [TestMethod]
        public void PrettifyJson_SingleJsonArray_FormatsWithIndentation()
        {
            string raw = "[\"first\",\"second\",3]";
            string formatted = raw.PrettifyJson();

            string expected = "[\r\n  \"first\",\r\n  \"second\",\r\n  3\r\n]";
            Assert.AreEqual(expected.Replace("\r\n", Environment.NewLine), formatted.Replace("\r\n", Environment.NewLine));
        }

        /// <summary>
        /// Verifies that PrettifyJson formats multiple consecutive top-level JSON tokens without truncating after the first token.
        /// </summary>
        [TestMethod]
        public void PrettifyJson_MultipleConsecutiveJsonTokens_FormatsAllTokensWithoutTruncating()
        {
            string raw = "{\"id\":1}{\"id\":2}";
            string formatted = raw.PrettifyJson();

            // Both objects should be present in the formatted output
            Assert.IsTrue(formatted.Contains("\"id\": 1"), "Formatted output must contain first token object.");
            Assert.IsTrue(formatted.Contains("\"id\": 2"), "Formatted output must contain second token object without truncation.");
        }

        /// <summary>
        /// Verifies that PrettifyJson throws a JsonException when input text has invalid JSON syntax.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void PrettifyJson_InvalidJson_ThrowsJsonException()
        {
            string invalid = "{ \"name\": \"unclosed string ";
            invalid.PrettifyJson();
        }

        /// <summary>
        /// Verifies that LineOfIndexOrDefault accurately calculates 1-based line numbers across CRLF, LF, and CR line endings.
        /// </summary>
        [TestMethod]
        public void LineOfIndexOrDefault_CalculatesAccurateLineNumbers()
        {
            string textCrlf = "Line 1\r\nLine 2\r\nLine 3";
            Assert.AreEqual(1, textCrlf.LineOfIndexOrDefault(0, LineEndings.CRLF));
            Assert.AreEqual(1, textCrlf.LineOfIndexOrDefault(5, LineEndings.CRLF));
            Assert.AreEqual(2, textCrlf.LineOfIndexOrDefault(8, LineEndings.CRLF));
            Assert.AreEqual(3, textCrlf.LineOfIndexOrDefault(18, LineEndings.CRLF));

            string textLf = "Line 1\nLine 2\nLine 3";
            Assert.AreEqual(1, textLf.LineOfIndexOrDefault(0, LineEndings.LF));
            Assert.AreEqual(2, textLf.LineOfIndexOrDefault(7, LineEndings.LF));
            Assert.AreEqual(3, textLf.LineOfIndexOrDefault(15, LineEndings.LF));
        }
    }
}
