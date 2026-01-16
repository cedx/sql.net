namespace Belin.Sql;

/// <summary>
/// Represents an SQL statement that is executed while connected to a data source.
/// </summary>
public sealed record Command {

	/// <summary>
	/// The parameters of the SQL statement.
	/// </summary>
	public ParameterCollection Parameters { get; init; } = [];

	/// <summary>
	/// The text of the SQL statement.
	/// </summary>
	public required string Text { get; init; }
}
