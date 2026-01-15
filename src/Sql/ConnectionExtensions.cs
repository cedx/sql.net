namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {
	// TODO (.NET 10) extension(IDbConnection connection)

	/// <summary>
	/// Creates a new command associated with the specified connection.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The newly created command.</returns>
	public static IDbCommand CreateCommand(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) {
		var command = connection.CreateCommand();
		command.CommandText = sql;
		command.CommandTimeout = options?.Timeout ?? 30;
		command.CommandType = options?.Type ?? CommandType.Text;
		command.Transaction = options?.Transaction;

		if (parameters is not null) foreach (var parameter in parameters) {
			var commandParameter = command.CreateParameter();
			commandParameter.ParameterName = parameter.Name;
			commandParameter.Value = parameter.Value;
			if (parameter.DbType is not null) commandParameter.DbType = parameter.DbType.Value;
			if (parameter.Direction is not null) commandParameter.Direction = parameter.Direction.Value;
			if (parameter.Precision is not null) commandParameter.Precision = parameter.Precision.Value;
			if (parameter.Scale is not null) commandParameter.Scale = parameter.Scale.Value;
			if (parameter.Size is not null) commandParameter.Size = parameter.Size.Value;
			command.Parameters.Add(commandParameter);
		}

		return command;
	}
}
