namespace Belin.Sql.Fixtures;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Represents a fictional character from a well-known saga.
/// </summary>
[Table("Characters", Schema = "main")]
public sealed class Character {

	/// <summary>
	/// The first name.
	/// </summary>
	[Column("firstName")]
	public string FirstName { get; set; } = "";

	/// <summary>
	/// The full name.
	/// </summary>
	[Column("fullName"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
	public string FullName { get; set; } = "";

	/// <summary>
	/// The character's gender.
	/// </summary>
	[Column("gender")]
	public CharacterGender Gender { get; set; } = CharacterGender.Human;

	/// <summary>
	/// The character's identifier.
	/// </summary>
	[Column("ID"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	/// <summary>
	/// The last name.
	/// </summary>
	[Column("lastName")]
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
