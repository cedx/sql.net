using module PSScriptAnalyzer

"Performing the static analysis of source code..."
Invoke-ScriptAnalyzer $PSScriptRoot, "src", "test" -Recurse
Test-ModuleManifest Sql.psd1 | Out-Null
