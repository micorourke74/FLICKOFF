# Repository sanity verification

The repository check is intentionally license-free. It checks project markers, tracked-file hygiene, JSON syntax, Unity cache exclusion, LFS policy, and `git diff --check`. It does not pretend to compile Unity.

Run:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\powershell\Test-RepositorySanity.ps1
```

Unity-specific validation is separate and must run through the exact installed editor using `tools/unity/Run-UnityBatch.ps1` or `tools/unity/Run-UnityTests.ps1`.
