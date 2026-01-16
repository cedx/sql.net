namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	/// <summary>
	/// Deletes the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to delete.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var identityColumn = Mapper.Instance.GetTable<T>().IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), identityColumn.GetValue(instance));
		return await ExecuteAsync(connection, builder.GetDeleteCommand<T>(), new(parameter), options) > 0;
	}

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
		return value is null || value is DBNull ? default : (T?) Mapper.Instance.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Checks whether an entity with the specified primary key exists.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
	public static async Task<bool> ExistsAsync<T>(this IDbConnection connection, object id, CommandOptions? options = null) where T: new() {
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), id);
		return await ExecuteScalarAsync<bool>(connection, builder.GetExistsCommand<T>(), new(parameter), options);
	}

	/// <summary>
	/// Finds an entity with the specified primary key.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> FindAsync<T>(this IDbConnection connection, object id, string[]? columns = null, CommandOptions? options = null) where T: new() {
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), id);
		return await QuerySingleOrDefaultAsync<T>(connection, builder.GetFindCommand<T>(columns ?? []), new(parameter), options);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<IEnumerable<ExpandoObject>> QueryAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, QueryOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryAsync<ExpandoObject>(connection, sql, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, QueryOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		var records = Mapper.Instance.CreateInstances<T>(await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken));
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of the first objects.</typeparam>
	/// <typeparam name="U">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The field from which to split and read the second object.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(T, U)>> QueryAsync<T, U>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, string splitOn = "Id", QueryOptions? options = null, CancellationToken cancellationToken = default) where T: new() where U: new() {
		var records = Mapper.Instance.CreateInstances<T, U>(await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

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
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QueryFirstAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
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
	public static async Task<T> QueryFirstAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
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
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QueryFirstOrDefaultAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
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
	public static async Task<T?> QueryFirstOrDefaultAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : default;
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
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QuerySingleAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
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
	public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
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
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QuerySingleOrDefaultAsync(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
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
	public static async Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, sql, parameters, options, cancellationToken);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
