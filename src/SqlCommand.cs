namespace Belin.Sql;

using System.Data;

/// <summary>
/// Represents an SQL statement that is executed while connected to a data source.
/// </summary>
/// <param name="text">The text of the SQL statement.</param>
public sealed class SqlCommand(string text) {

	/// <summary>
	/// Value indicating whether to prevent from buffering the rows in memory.
	/// </summary>
	public bool Stream { get; set; }

	/// <summary>
	/// The text of the SQL statement.
	/// </summary>
	public string Text { get; set; } = text;

	/// <summary>
	/// The wait time, in seconds, before terminating the attempt to execute the command and generating an error.
	/// </summary>
	public int Timeout { get; set; } = 30;

	/// <summary>
	/// The transaction within which the command executes.
	/// </summary>
	public IDbTransaction? Transaction { get; set; }

	/// <summary>
	/// Value indicating how the command is interpreted.
	/// </summary>
	public CommandType Type { get; set; } = CommandType.Text;

	/// <summary>
	/// Creates a new command from the specified text.
	/// </summary>
	/// <param name="text">The text providing the SQL statement.</param>
	/// <returns>The command corresponding to the specified text.</returns>
	public static implicit operator SqlCommand(string text) => new(text);

	/// <summary>
	/// Converts this command into an <see cref="IDbCommand"/> object.
	/// </summary>
	/// <param name="connection">The connection to associate with the created command.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The <see cref="IDbCommand"/> object corresponding to this command.</returns>
	internal IDbCommand ToDbParameter(IDbConnection connection, SqlParameterCollection? parameters = null) {
		var command = connection.CreateCommand();
		command.CommandText = Text;
		command.CommandTimeout = Timeout;
		command.CommandType = Type;
		command.Transaction = Transaction;
		foreach (var parameter in parameters ?? []) command.Parameters.Add(parameter.ToDbParameter(command));
		return command;
	}
}
