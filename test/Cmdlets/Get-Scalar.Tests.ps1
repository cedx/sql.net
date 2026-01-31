using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Tests the features of the `Get-Scalar` cmdlet.
#>
Describe "Get-Scalar" {
	BeforeAll {
		Import-Module "$PSScriptRoot/../../Sql.psd1"
		Import-Module "$PSScriptRoot/../../bin/System.Data.SQLite.dll"
	}

	BeforeEach {
		[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
		$connection = [System.Data.SQLite.SQLiteConnection]::new("DataSource=$PSScriptRoot/../../res/Database.sqlite")
	}

	AfterEach {
		$connection.Close()
	}

	It "should return the single value produced by the SQL query" {
		$sql = "SELECT COUNT(*) FROM Characters WHERE Gender = @Gender"
		$value = Get-SqlScalar $connection -Command $sql -Parameters @{ Gender = "Balrog" }
		$value | Should -Be 2

		$sql = "SELECT tbl_name FROM sqlite_schema WHERE type = 'table' AND name = @Name"
		$value = Get-SqlScalar $connection -Command $sql -Parameters @{ Name = "Characters" }
		$value | Should -BeExactly Characters
	}
}
