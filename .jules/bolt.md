
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.

## 2026-11-01 - ⚡ Bolt: Cache Dialog Type Resolutions
**Learning:** `DialogService.GetDialog` uses `Assembly.GetEntryAssembly().GetTypes()` on every invocation, causing significant reflection overhead. Additionally, `Assembly.GetEntryAssembly()` can be null in test environments causing `NullReferenceException`.
**Action:** Implemented a thread-safe static `Dictionary<string, Type>` cache initialized with double-check locking, and added fallback to `typeof(DialogService).Assembly`.
