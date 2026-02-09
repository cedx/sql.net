namespace Belin.Sql;

using System.Data;

/// <summary>
/// Represents an SQL statement that is executed while connected to a data source.
/// </summary>
/// <param name="Text">The text of the SQL statement.</param>
/// <param name="Parameters">The parameters of the SQL statement.</param>
public sealed record Command(string Text, ParameterCollection Parameters) {

	/// <summary>
	/// Converts this command into an <see cref="IDbCommand"/> object.
	/// </summary>
	/// <param name="connection">The onnectio to associate with the created command.</param>
	/// <returns>The <see cref="IDbCommand"/> object corresponding to this command.</returns>
	internal IDbCommand ToDbParameter(IDbConnection connection) {
		var command = connection.CreateCommand();
		command.CommandText = Text;
		command.CommandTimeout = Timeout;
		command.CommandType = Type;
		command.Transaction = Transaction;
		foreach (var parameter in Parameters) command.Parameters.Add(parameter.ToDbParameter(command));
		return command;
	}
}
