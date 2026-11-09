
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.

## 2026-11-09 - ⚡ Bolt: Optimized FileHistoryService.ApplySegmentRetention
**Learning:** FileHistoryService.ApplySegmentRetention had an O(N^2) bottleneck caused by repeatedly calling `manifest.Versions.Sum()` and fully scanning for baseline indices within the retention loop. This caused significant latency on large history manifests.
**Action:** Optimized the loop by pre-calculating the sum outside the loop and manually subtracting removed bytes, and short-circuited the baseline search once the first two baselines are found.
