namespace Belin.Sql;

using System.Data;

/// <summary>
/// Represents an SQL statement that is executed while connected to a data source.
/// </summary>
/// <param name="text">The text of the SQL statement.</param>
/// <param name="parameters">The parameters of the SQL statement.</param>
public sealed class SqlCommand(string text, SqlParameterCollection? parameters = null) {

	/// <summary>
	/// The parameters of the SQL statement.
	/// </summary>
	public SqlParameterCollection Parameters { get; set; } = parameters ?? [];

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
	/// Deconstructs this instance by <see cref="Text"/> and <see cref="Parameters"/>.
	/// </summary>
	/// <param name="text">When this method returns, contains the <see cref="Text"/> value of this instance.</param>
	/// <param name="parameters">When this method returns, contains the <see cref="Parameters"/> value of this instance.</param>
	public void Deconstruct(out string text, out SqlParameterCollection parameters) {
		text = Text;
		parameters = Parameters;
	}

	/// <summary>
	/// Converts this command into an <see cref="IDbCommand"/> object.
	/// </summary>
	/// <param name="connection">The onnectio to associate with the created command.</param>
	/// <returns>The <see cref="IDbCommand"/> object corresponding to this command.</returns>
	public IDbCommand ToDbParameter(IDbConnection connection) {
		var command = connection.CreateCommand();
		command.CommandText = Text;
		command.CommandTimeout = Timeout;
		command.CommandType = Type;
		command.Transaction = Transaction;
		foreach (var parameter in Parameters) command.Parameters.Add(parameter.ToDbParameter(command));
		return command;
	}
}
