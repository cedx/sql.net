using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Tests the features of the `Get-Single` cmdlet.
#>
Describe "Get-Single" {
	BeforeAll {
		Import-Module "$PSScriptRoot/../../Sql.psd1"
		Import-Module "$PSScriptRoot/../../bin/System.Data.SQLite.dll"
		. "$PSScriptRoot/../Fixtures/Character.ps1"
	}

	BeforeEach {
		[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
		$connection = [System.Data.SQLite.SQLiteConnection]::new("DataSource=$PSScriptRoot/../../res/Database.sqlite")
	}

	AfterEach {
		$connection.Close()
	}

	It "should return the single record produced by the SQL query" {
		$sql = "SELECT * FROM Characters WHERE FullName = @FullName"
		$record = Get-SqlSingle $connection -As ([Character]) -Command $sql -Parameters @{ FullName = "Saruman" }
		$record.FirstName | Should -BeExactly "Saruman"
		$record.Gender | Should -Be ([CharacterGender]::Istari)
	}
}
