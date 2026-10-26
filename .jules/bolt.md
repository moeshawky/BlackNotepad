
## 2026-10-10 - ⚡ Bolt: Optimized GoTo Line Scanning
**Learning:** The legacy `GoTo` implementation used an O(N) character-by-character scan which was slow on very large documents (~10+ seconds for 1M lines). `IndexOf` provides a much faster native string search.
**Action:** Replaced the O(N) manual scan with `string.IndexOf(lineEndingChar)` which yields an 11x performance speedup while precisely preserving bug-for-bug edge case behavior using a minimal fallback.
## 2026-10-26 - ⚡ Bolt: Optimized Dialog Type Reflection Scanning
**Learning:** Resolving dialog types dynamically using `Assembly.GetTypes()` inside `GetDialog` scans the entire entry assembly via reflection every time a dialog opens, adding unnecessary UI latency. Additionally, in test environments, `Assembly.GetEntryAssembly()` can be null, throwing unhandled exceptions.
**Action:** Cache the discovered types in a thread-safe `Dictionary<string, Type>` initialized via double-check locking to eliminate recurring reflection overhead. Always provide a fallback to `Assembly.GetExecutingAssembly()` for environment resilience.
