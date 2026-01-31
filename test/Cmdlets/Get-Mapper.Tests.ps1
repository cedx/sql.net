<#
.SYNOPSIS
	Tests the features of the `Get-Mapper` cmdlet.
#>
Describe "Get-Mapper" {
	BeforeAll {
		Import-Module "$PSScriptRoot/../../Sql.psd1"
		. "$PSScriptRoot/../Fixtures/Character.ps1"
	}

	Describe "CreateInstance()" {
		It "should create instances of the requested type" {
			$properties = @{
				Class = "Bard/minstrel"
				FirstName = "Cédric"
				Gender = [CharacterGender]::Balrog.ToString()
				LastName = $null
			}

			$instance = (Get-SqlMapper).CreateInstance($properties)
			$instance.GetType().FullName | Should -BeExactly System.Dynamic.ExpandoObject
			$instance.Class | Should -BeExactly "Bard/minstrel"
			$instance.FirstName | Should -BeExactly "Cédric"
			$instance.Gender | Should -BeExactly ([CharacterGender]::Balrog.ToString())
			$instance.LastName | Should -Be $null

			$character = (Get-SqlMapper).CreateInstance[Character]($properties)
			$character.GetType().Name | Should -BeExactly Character
			$character.FirstName | Should -BeExactly "Cédric"
			$character.Gender | Should -Be ([CharacterGender]::Balrog)
			$character.LastName | Should -Be $null
		}
	}

	Describe "GetTable()" {
		It "should return information about the tables and columns of an entity type" {
			$table = (Get-SqlMapper).GetTable[Character]()
			$table.Schema | Should -BeExactly main
			$table.Name | Should -BeExactly Characters
			$table.Type | Should -Be ([Character])

			$table.Columns.Keys | Should -HaveCount 5
			$table.IdentityColumn | Should -Be $table.Columns["Id"]
			$table.Columns["Gender"].Type | Should -Be ([CharacterGender])
			$table.Columns["LastName"].Type | Should -Be ([string])

			$table.Columns["FirstName"].CanWrite | Should -BeTrue
			$table.Columns["FullName"].IsComputed | Should -BeTrue
			$table.Columns["Id"].IsIdentity | Should -BeTrue
		}
	}
}
