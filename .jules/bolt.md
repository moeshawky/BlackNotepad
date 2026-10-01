## 2026-10-01 - [Optimize JSON formatting with token streaming]
**Learning:** Parsing large JSON strings into an intermediate `dynamic` object graph using `JsonConvert.DeserializeObject` causes massive object allocations and GC pressure, stalling the application.
**Action:** Use `JsonTextReader` and `JsonTextWriter` to stream JSON tokens directly to the formatted output, eliminating intermediate allocations. Explicitly set `DateParseHandling.None` on the reader to prevent dates from being mutated during the stream.
