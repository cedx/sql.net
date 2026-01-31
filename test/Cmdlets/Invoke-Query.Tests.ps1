using namespace System.Diagnostics.CodeAnalysis

<#
.SYNOPSIS
	Tests the features of the `Invoke-Query` cmdlet.
#>
Describe "Invoke-Query" {
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

	It "should return the records produced by the SQL query" {
		$sql = "SELECT * FROM Characters WHERE Gender = @Gender ORDER BY FullName"
		$records = Invoke-SqlQuery $connection -As ([Character]) -Command $sql -Parameters @{ Gender = "Elf" }
		$records | Should -HaveCount 3

		$elrond = $records[0]
		$elrond.FullName | Should -BeExactly "Elrond"
		$elrond.Gender | Should -Be ([CharacterGender]::Elf)

		$galadriel = $records[1]
		$galadriel.FullName | Should -BeExactly "Galadriel"
		$galadriel.Gender | Should -Be ([CharacterGender]::Elf)
	}
}
