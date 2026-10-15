## 2026-10-15 - PrettifyJsonText Memory Pressure
**Learning:** Using `JsonConvert.DeserializeObject` and `JsonConvert.SerializeObject` for formatting JSON creates massive intermediate object graphs.
**Action:** Use `JsonTextReader` and `JsonTextWriter` for memory-efficient token streaming, and ensure `DateParseHandling = DateParseHandling.None` is set to avoid date mutation.
