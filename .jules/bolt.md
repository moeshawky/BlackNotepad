
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.

## 2026-11-11 - ⚡ Bolt: Dialog type reflection caching
**Learning:** `DialogService.GetDialog` was dynamically calling `Assembly.GetEntryAssembly().GetTypes()` on every dialog resolution. This causes unnecessary overhead on each UI dialog creation, and fails in unit tests where `GetEntryAssembly` is null.
**Action:** Implemented a double-check locked thread-safe static `Dictionary<string, Type>` cache that computes the types once, and added fallback to `Assembly.GetExecutingAssembly()` for test environments.
