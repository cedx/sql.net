<#
.SYNOPSIS
	Represents a fictional character from a well-known saga.
#>
class Character {

	<#
	.SYNOPSIS
		The first name.
	#>
	[string] $FirstName = ""

	<#
	.SYNOPSIS
		The character's gender.
	#>
	[CharacterGender] $Gender = [CharacterGender]::Human

	<#
	.SYNOPSIS
		The last name.
	#>
	[string] $LastName = ""
}

<#
.SYNOPSIS
	Defines the gender of a character.
#>
enum CharacterGender {
	Balrog
	DarkLord
	Dwarf
	Elf
	Hobbit
	Human
	Istari
}
