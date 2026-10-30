
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.
## 2026-10-30 - ⚡ Bolt: Cached Dialog Reflection
**Learning:** `DialogService.GetDialog` was using an expensive reflection scan (`Assembly.GetTypes()`) every time a dialog was opened, which caused unnecessary overhead. It also crashed test environments when `GetEntryAssembly()` returned null.
**Action:** Optimized using a thread-safe static `Dictionary<string, Type>` cache initialized with double-check locking, and fallback to `Assembly.GetExecutingAssembly()`. This removes the reflection bottleneck and prevents NullReferenceException bugs in tests.
