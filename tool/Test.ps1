using module ./Cmdlets.psm1

"Running the test suite..."
Invoke-DotNetTest -Settings "$PSScriptRoot/../.runsettings"
Invoke-PowerShellTest test -EnableExit
