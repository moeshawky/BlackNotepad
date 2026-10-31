
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.
## 2026-10-31 - Optimize FileHistoryService.ApplySegmentRetention

**Learning:** Calling `IEnumerable.Sum` inside a `while (true)` loop over a potentially large collection (e.g. `manifest.Versions`) results in an O(N^2) operation, causing severe performance bottlenecks.
**Action:** Pre-calculate sums outside of loops and adjust them incrementally (O(N) operation), and optimize collection scans by breaking early when the required information is found, avoiding full collection scans.
