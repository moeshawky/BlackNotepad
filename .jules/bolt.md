## 2026-10-24 - MainViewModel Replace Optimization
**Learning:** In C#, replacing text at a known index via `string.Compare` and `.Remove().Insert()` avoids the O(N) full-document scan and the significant GC pressure of multiple `Substring` allocations.
**Action:** When replacing text at a known position, validate the match with `string.Compare(source, index, target, 0, length)` and use `.Remove(index, length).Insert(index, replacement)` instead of full-string scans and substring concatenation.
## 2026-10-24 - MainViewModel Replace Memory Optimization Correction
**Learning:** In C#, replacing text via `content.Remove().Insert()` on large documents forces two massive string allocations, actively worsening memory pressure compared to the original `Substring` and `string.Concat` approach. Additionally, directly mutating UI-bound properties like `IndexOfCaret` in internal logic can trigger unintended `PropertyChanged` side effects.
**Action:** Reverted the string mutation to use `Substring` and `string.Concat` (which allocates fewer strings) while maintaining the `string.Compare` O(N) scan optimization. Always avoid assigning UI-bound properties in background logic.
