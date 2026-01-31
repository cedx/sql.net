using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Tests the features of the `Get-First` cmdlet.
#>
Describe "Get-First" {
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

	It "should return the first record produced by the SQL query" {
		$sql = "SELECT * FROM Characters WHERE FullName = @FullName"
		$record = Get-SqlFirst $connection -As ([Character]) -Command $sql -Parameters @{ FullName = "Sauron" }
		$record.FirstName | Should -BeExactly "Sauron"
		$record.Gender | Should -Be ([CharacterGender]::DarkLord)
	}
}
