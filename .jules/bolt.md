
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.

## 2026-11-06 - ⚡ Bolt: Optimized FileHistoryService.ApplySegmentRetention
**Learning:** Calling `manifest.Versions.Sum(v => v.StoredSizeBytes)` inside a `while` loop caused an O(N^2) bottleneck for large historical versions, combined with scanning all baseline indices.
**Action:** Pre-calculate `manifest.Versions.Sum` outside of the retention loop and dynamically update it, and short-circuit the baseline index scan early.
