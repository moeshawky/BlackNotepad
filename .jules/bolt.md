## 2024-05-22 - [Optimizing Legacy File I/O]
**Learning:** Legacy code might have unique business logic (like line ending detection) mixed with inefficient I/O. When optimizing `StreamReader.Read` loops to `ReadToEnd`, one must carefully extract and replicate the logical conditions on the resulting string, rather than just replacing the I/O call.
**Action:** When replacing stream loops with bulk reads, map out the exact state transitions of the loop to ensure the post-processing of the string yields identical results.

## 2024-05-22 - [CI/Test Environment Limitations]
**Learning:** The CI environment for this legacy .NET project seems fragile regarding new test files or specific test logic (possibly `Path.GetTempFileName` or `System.Threading.Tasks` in tests). Adding new tests blocked the submission of the valid performance fix.
**Action:** When CI fails on new tests for a legacy codebase without clear logs, prioritize landing the fix if it has been verified via other means (e.g., simulation scripts), and defer test infrastructure improvements to a separate task.
