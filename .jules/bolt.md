
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.
## 2026-10-28 - [Optimize DialogService reflection]
**Learning:** DialogService.GetDialog repeatedly scanned the entry assembly using Assembly.GetEntryAssembly().GetTypes(), which is expensive O(N) reflection overhead and creates many allocations.
**Action:** Used a thread-safe static Dictionary<string, Type> initialized via double-check locking to turn O(N) scans into an O(1) cache lookup, while handling null entry assembly for test environments.
