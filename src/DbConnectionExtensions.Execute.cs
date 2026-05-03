namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The number of rows affected.</returns>
	public static int Execute(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		return dbCommand.ExecuteNonQuery();
	}

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The number of rows affected.</returns>
	public static async Task<int> ExecuteAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		return await dbCommand.ExecuteNonQueryAsync(cancellationToken);
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first column of the first row.</returns>
	public static object? ExecuteScalar(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		ExecuteScalar<object>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<object?> ExecuteScalarAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await ExecuteScalarAsync<object>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first column of the first row.</returns>
	public static T? ExecuteScalar<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		var value = dbCommand.ExecuteScalar();
		return value is null || value is DBNull ? default : (T?) SqlMapper.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<T?> ExecuteScalarAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		var value = await dbCommand.ExecuteScalarAsync(cancellationToken);
		return value is null || value is DBNull ? default : (T?) SqlMapper.ChangeType(value, typeof(T));
	}
}
