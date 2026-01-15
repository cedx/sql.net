@{
	DefaultCommandPrefix = "Sql"
	ModuleVersion = "3.0.0"
	PowerShellVersion = "7.5"
	RootModule = "bin/Belin.Sql.Cmdlets.dll"

	Author = "Cédric Belin <cedx@outlook.com>"
	CompanyName = "Cedric-Belin.fr"
	Copyright = "© Cédric Belin"
	Description = "A simple micro-ORM, based on ADO.NET and data annotations."
	GUID = "d2b1c123-e1bc-4cca-84c5-af102244e3c5"

	AliasesToExport = @()
	FunctionsToExport = @()
	VariablesToExport = @()

	CmdletsToExport = @(
		"Approve-Transaction"
		"Close-Connection"
		"Deny-Transaction"
		"Find-Object"
		"Get-First"
		"Get-Mapper"
		"Get-Scalar"
		"Get-Single"
		"Invoke-NonQuery"
		"Invoke-Query"
		"Invoke-Reader"
		"New-Command"
		"New-Connection"
		"New-Parameter"
		"New-Transaction"
		"Remove-Object"
		"Test-Object"
	)

	RequiredAssemblies = @(
		"bin/Belin.Sql.dll"
	)

	PrivateData = @{
		PSData = @{
			LicenseUri = "https://github.com/cedx/sql.net/blob/main/License.md"
			ProjectUri = "https://github.com/cedx/sql.net"
			ReleaseNotes = "https://github.com/cedx/sql.net/releases"
			Tags = "ado.net", "data", "database", "mapper", "mapping", "orm", "query", "sql"
		}
	}
}
