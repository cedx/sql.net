using namespace System.ComponentModel.DataAnnotations.Schema

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
		The full name.
	#>
	[DatabaseGenerated([DatabaseGeneratedOption]::Computed)]
	[string] $FullName = ""

	<#
	.SYNOPSIS
		The character's gender.
	#>
	[CharacterGender] $Gender = [CharacterGender]::Human

	<#
	.SYNOPSIS
		The character's identifier.
	#>
	[int] $Id = 0

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
