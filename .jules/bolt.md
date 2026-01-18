## 2026-01-18 - [Legacy IO Anti-pattern]
**Learning:** The codebase contained a highly inefficient file reading loop that processed files character-by-character using `StreamReader.Read()` and `Peek()`, ostensibly for line-ending detection. This is orders of magnitude slower than `ReadToEnd()`.
**Action:** When working on IO operations in this codebase, immediately verify if legacy manual buffering or character processing loops are used and replace them with standard stream methods while preserving logic.
