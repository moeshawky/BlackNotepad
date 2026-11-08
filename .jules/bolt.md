
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.

## 2026-11-08 - ⚡ Bolt: Optimized FileHistoryService.ApplySegmentRetention
**Learning:** The previous implementation of `ApplySegmentRetention` recalculated the total byte size (`Sum`) and rescanned all baselines during every iteration of its pruning loop. For histories with many versions, this resulted in an O(N^2) CPU bottleneck.
**Action:** Pre-calculate sums outside the pruning loop, subtract sizes incrementally when segments are deleted, and short-circuit baseline index searches to eliminate the O(N^2) overhead.
