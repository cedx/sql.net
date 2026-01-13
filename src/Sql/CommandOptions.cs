namespace Belin.Sql;

using System.Data;

/// <summary>
/// Defines the options of a SQL command.
/// </summary>
public record CommandOptions {

	/// <summary>
	/// The wait time, in seconds, before terminating the attempt to execute the command and generating an error.
	/// </summary>
	public int Timeout { get; init; } = 30;

	/// <summary>
	/// The transaction within which the command executes.
	/// </summary>
	public IDbTransaction? Transaction { get; init; }

	/// <summary>
	/// Value indicating how the command is interpreted.
	/// </summary>
	public CommandType Type { get; init; } = CommandType.Text;
}
