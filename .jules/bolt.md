## 2024-05-22 - [Optimizing Legacy File I/O]
**Learning:** Legacy code might have unique business logic (like line ending detection) mixed with inefficient I/O. When optimizing `StreamReader.Read` loops to `ReadToEnd`, one must carefully extract and replicate the logical conditions on the resulting string, rather than just replacing the I/O call.
**Action:** When replacing stream loops with bulk reads, map out the exact state transitions of the loop to ensure the post-processing of the string yields identical results.
