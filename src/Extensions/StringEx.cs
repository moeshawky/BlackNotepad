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

using Newtonsoft.Json;
using Savaged.BlackNotepad.Lookups;
using System;
using System.IO;

namespace Savaged.BlackNotepad.Extensions
{
    /// <summary>
    /// Provides extension methods for strings, including position queries and JSON formatting.
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// Calculates the 1-based line number corresponding to a character index within a string.
        /// </summary>
        /// <param name="self">The source text to inspect.</param>
        /// <param name="index">The zero-based character index.</param>
        /// <param name="lineEnding">The line ending style used in the text.</param>
        /// <returns>The 1-based line number containing the character index, or 1 if the string is null or empty.</returns>
        public static int LineOfIndexOrDefault(
            this string self, int index, LineEndings lineEnding)
        {
            if (string.IsNullOrEmpty(self))
            {
                return 1;
            }

            var lineEndingStr = "\r";
            if (lineEnding == LineEndings.LF)
            {
                lineEndingStr = "\n";
            }
            else if (lineEnding == LineEndings.CRLF)
            {
                lineEndingStr = "\r\n";
            }

            // Counts separator occurrences at or before index instead of
            // splitting the document. Proven equivalent to the prior
            // Split-based walk: Split returns i + 1 at the first line whose
            // end offset exceeds index, which is exactly 1 plus the number
            // of separators starting at or before index. O(n) time, O(1)
            // allocations instead of O(n) substring copies.
            var line = 1;
            var position = self.IndexOf(lineEndingStr, StringComparison.Ordinal);
            while (position >= 0 && position <= index)
            {
                line++;
                position = self.IndexOf(
                    lineEndingStr,
                    position + lineEndingStr.Length,
                    StringComparison.Ordinal);
            }

            return line;
        }

        /// <summary>
        /// Formats and indents raw JSON text, supporting multiple consecutive top-level tokens or JSON documents.
        /// </summary>
        /// <param name="json">The raw JSON string to prettify.</param>
        /// <returns>The indented and formatted JSON string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
        /// <exception cref="JsonException">Thrown when <paramref name="json"/> contains invalid JSON syntax.</exception>
        public static string PrettifyJson(this string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return json;
            }

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader) { SupportMultipleContent = true, DateParseHandling = DateParseHandling.None })
            using (var stringWriter = new StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented })
                {
                    while (jsonReader.Read())
                    {
                        jsonWriter.WriteToken(jsonReader);
                    }
                }
                return stringWriter.ToString();
            }
        }
    }
}
