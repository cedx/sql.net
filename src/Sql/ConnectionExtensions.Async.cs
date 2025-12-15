namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The number of rows affected.</returns>
	public static async Task<int> ExecuteAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, sql, parameters, options);
		return await command.ExecuteNonQueryAsync(cancellationToken);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a data reader.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The data reader that can be used to access the results.</returns>
	public static async Task<IDataReader> ExecuteReaderAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, sql, parameters, options);
		return await command.ExecuteReaderAsync(cancellationToken);
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<object?> ExecuteScalarAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		ExecuteScalarAsync<object>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first column of the first row.</returns>
	public static async Task<T?> ExecuteScalarAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) {
		if (connection.State == ConnectionState.Closed) await ((DbConnection) connection).OpenAsync(cancellationToken);
		using var command = (DbCommand) CreateCommand(connection, sql, parameters, options);
		var value = await command.ExecuteScalarAsync(cancellationToken);
		return value is null || value is DBNull ? default : (T?) mapper.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: class, new() =>
		mapper.CreateInstances<T>(await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken));

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static async Task<dynamic> QueryFirstAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryFirstAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static async Task<T> QueryFirstAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: class, new() {
		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		return reader.Read() ? mapper.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static async Task<dynamic?> QueryFirstOrDefaultAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryFirstOrDefaultAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QueryFirstOrDefaultAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: class, new() {
		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		return reader.Read() ? mapper.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static async Task<dynamic> QuerySingleAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QuerySingleAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: class, new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = mapper.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static async Task<dynamic?> QuerySingleOrDefaultAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QuerySingleOrDefaultAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: class, new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = mapper.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
