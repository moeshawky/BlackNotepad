## 2026-09-29 - [JSON Formatting GC Pressure]
**Learning:** Deserializing large JSON documents into intermediate object graphs (e.g., dynamic) solely for re-serialization causes massive unnecessary memory allocations and UI thread hangs (GC pressure).
**Action:** Always prefer token streaming (JsonTextReader to JsonTextWriter) for transforming JSON structures where DOM manipulation is not required.
