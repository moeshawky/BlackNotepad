## 2024-05-23 - Regex misuse in Find/Replace
**Learning:** The application was using `Regex.Replace` for all replacements, including simple literal ones. This introduced performance overhead and correctness bugs (regex metacharacters in search queries were interpreted as patterns).
**Action:** Always check if a simple string replacement is sufficient before using Regex. When using Regex for user input, always use `Regex.Escape`.
