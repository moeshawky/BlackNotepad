
## 2026-10-08 - Replace 100ms DispatcherTimer with Event-Driven Sync
**Learning:** In WPF, relying on a `DispatcherTimer` firing every 100ms to synchronize line counts and scroll offsets keeps the UI thread permanently warm, which is an anti-pattern.
**Action:** Always prefer hooking into specific events like `TextChanged`, `SizeChanged`, and `ScrollViewer.ScrollChanged` to update view-model states precisely when they change, avoiding unnecessary continuous polling.
