<#
.SYNOPSIS
	Tests the features of the `Get-Mapper` cmdlet.
#>
Describe "Get-Mapper" {
	BeforeAll {
		Import-Module "$PSScriptRoot/../../Sql.psd1"
		. "$PSScriptRoot/../Fixtures/Character.ps1"
	}

	It "should create instances of the requested type" {
		$mapper = Get-SqlMapper
		$properties = @{
			Class = "Bard/minstrel"
			FirstName = "Cédric"
			Gender = [CharacterGender]::Balrog.ToString()
			LastName = $null
		}

		$instance = $mapper.CreateInstance($properties)
		$instance.GetType().FullName | Should -BeExactly System.Dynamic.ExpandoObject
		$instance.Class | Should -BeExactly "Bard/minstrel"
		$instance.FirstName | Should -BeExactly "Cédric"
		$instance.Gender | Should -BeExactly ([CharacterGender]::Balrog.ToString())
		$instance.LastName | Should -Be $null

		$character = $mapper.CreateInstance[Character]($properties)
		$character.GetType().Name | Should -BeExactly Character
		$character.FirstName | Should -BeExactly "Cédric"
		$character.Gender | Should -Be ([CharacterGender]::Balrog)
		$character.LastName | Should -Be $null
	}
}
