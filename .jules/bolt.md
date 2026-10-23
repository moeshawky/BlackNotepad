## 2026-10-23 - Optimize LineNumbersControl rendering with StringBuilder
**Learning:** Large dynamic text strings in WPF UI controls (like gutter line numbers) shouldn't be built with array allocations and string joining (`ToString()` and `string.Join`), as it causes massive O(N) heap allocations.
**Action:** Stream directly into a pre-sized `StringBuilder` to significantly reduce GC pressure and speed up UI rendering.
