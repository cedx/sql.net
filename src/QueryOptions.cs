namespace Belin.Sql;

/// <summary>
/// Defines the options of a SQL query.
/// </summary>
public record QueryOptions: CommandOptions {

	/// <summary>
	/// Value indicating whether to buffer the rows in memory.
	/// </summary>
	public bool Buffered { get; init; } = true;
}
