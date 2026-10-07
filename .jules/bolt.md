## 2026-10-07 - Optimize MainViewModel.Replace
**Learning:** An optimization opportunity exists in `MainViewModel.Replace`: it can use `string.Compare` with string indices to validate matches instead of an O(N) full document scan, and `string.Remove(...).Insert(...)` for faster string mutation compared to allocating multiple substrings and using `string.Concat`.
**Action:** When validating a substring match at a known index, always use `string.Compare` with index offsets to avoid full-string scans, and use `Remove/Insert` for efficient string replacement.
