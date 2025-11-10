@{
	DefaultCommandPrefix = "Sql"
	ModuleVersion = "0.1.0"
	PowerShellVersion = "7.4"
	RootModule = "src/Main.psm1"

	Author = "Cédric Belin <cedx@outlook.com>"
	CompanyName = "Cedric-Belin.fr"
	Copyright = "© Cédric Belin"
	Description = "A high performance micro-ORM supporting MariaDB, PostgreSQL, SQL Server, SQLite and more."
	GUID = "73f259d4-477b-4162-8bc7-af02221cef9c"

	AliasesToExport = @()
	CmdletsToExport = @()
	VariablesToExport = @()

	FunctionsToExport = @(
		"Close-Connection"
		"Get-First"
		"Get-Scalar"
		"Get-Single"
		"Get-Version"
		"Invoke-NonQuery"
		"Invoke-Query"
		"Invoke-Reader"
		"New-Connection"
	)

	NestedModules = @(
		"src/Close-Connection.psm1"
		"src/Get-First.psm1"
		"src/Get-Scalar.psm1"
		"src/Get-Single.psm1"
		"src/Invoke-NonQuery.psm1"
		"src/Invoke-Query.psm1"
		"src/Invoke-Reader.psm1"
		"src/New-Connection.psm1"
	)

	PrivateData = @{
		PSData = @{
			LicenseUri = "https://github.com/cedx/sql.ps1/blob/main/License.md"
			ProjectUri = "https://github.com/cedx/sql.ps1"
			ReleaseNotes = "https://github.com/cedx/sql.ps1/releases"
			Tags = "ado.net", "client", "database", "orm", "query", "sql"
		}
	}
}
