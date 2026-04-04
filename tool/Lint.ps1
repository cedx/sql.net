"Performing the static analysis of source code..."
Import-Module PSScriptAnalyzer
$PSScriptRoot, "test" | Invoke-ScriptAnalyzer -Recurse
Test-ModuleManifest Sql.psd1 | Out-Null
