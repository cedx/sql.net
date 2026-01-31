namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The number of rows affected.</returns>
	public static int Execute(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, text, parameters, options);
		return command.ExecuteNonQuery();
	}

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The number of rows affected.</returns>
	public static async Task<int> ExecuteAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, text, parameters, options);
		return await command.ExecuteNonQueryAsync(cancellationToken);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a data reader.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The data reader that can be used to access the results.</returns>
	public static IDataReader ExecuteReader(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, text, parameters, options);
		return command.ExecuteReader();
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a data reader.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The data reader that can be used to access the results.</returns>
	public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, text, parameters, options);
		return await command.ExecuteReaderAsync(cancellationToken);
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first column of the first row.</returns>
	public static object? ExecuteScalar(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		ExecuteScalar<object>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<object?> ExecuteScalarAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		ExecuteScalarAsync<object>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first column of the first row.</returns>
	public static T? ExecuteScalar<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, text, parameters, options);
		var value = command.ExecuteScalar();
		return value is null || value is DBNull ? default : (T?) Mapper.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<T?> ExecuteScalarAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, text, parameters, options);
		var value = await command.ExecuteScalarAsync(cancellationToken);
		return value is null || value is DBNull ? default : (T?) Mapper.ChangeType(value, typeof(T));
	}
}
