## 2024-05-22 - Legacy File Reading Quirk
**Learning:** The legacy `ReadFile` implementation had a bug where empty files resulted in a string containing `\uFFFF`. This was likely unintended but was the behavior due to casting `StreamReader.Read()` result (-1) to char.
**Action:** When optimizing legacy I/O code, check specifically for empty file behavior as it might be undefined or buggy in the original implementation.
