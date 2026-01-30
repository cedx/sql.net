namespace Belin.Sql.Fixtures;

/// <summary>
/// Represents a fictional character from a well-known saga.
/// </summary>
public sealed class Character {

	/// <summary>
	/// The first name.
	/// </summary>
	public string FirstName { get; set; } = "";

	/// <summary>
	/// The character's gender.
	/// </summary>
	public CharacterGender Gender { get; set; } = CharacterGender.Human;

	/// <summary>
	/// The last name.
	/// </summary>
	public string LastName { get; set; } = "";
}

/// <summary>
/// Defines the gender of a character.
/// </summary>
public enum CharacterGender {
	Balrog,
	DarkLord,
	Dwarf,
	Elf,
	Hobbit,
	Human,
	Istari
}
