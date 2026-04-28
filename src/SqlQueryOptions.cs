namespace Belin.Sql;

/// <summary>
/// Defines the options of a SQL query.
/// </summary>
public record SqlQueryOptions: SqlCommandOptions {

	/// <summary>
	/// Value indicating whether to prevent from buffering the rows in memory.
	/// </summary>
	public bool Stream { get; init; } = false;
}
