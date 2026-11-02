
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.
## 2026-11-02 - ⚡ Bolt: Optimized FileHistoryService Segment Retention
**Learning:** The previous `ApplySegmentRetention` implementation recalculated total retention bytes `manifest.Versions.Sum(v => v.StoredSizeBytes)` and rebuilt the `baselineIndices` list on every loop iteration, causing an O(N^2) CPU bottleneck when dealing with thousands of history versions.
**Action:** Lift the `totalBytes` sum out of the loop and decrement it incrementally as segments are pruned. Replace the full `baselineIndices` list allocation with a short-circuit scan to find just the first `firstSegmentEnd`. This provides a massive speedup on large histories.
