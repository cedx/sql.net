namespace Belin.Sql;

using System.Data;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	/// <summary>
	/// Deletes the specified entity.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to be deleted.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public static bool Delete<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var identityColumn = Mapper.Instance.GetTable<T>().IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), identityColumn.GetValue(instance));
		return Execute(connection, builder.GetDeleteCommand<T>(), new(parameter), options) > 0;
	}

	/// <summary>
	/// Executes a parameterized SQL statement.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The number of rows affected.</returns>
	public static int Execute(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, sql, parameters, options);
		return command.ExecuteNonQuery();
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a data reader.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The data reader that can be used to access the results.</returns>
	public static IDataReader ExecuteReader(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, sql, parameters, options);
		return command.ExecuteReader();
	}

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first column of the first row.</returns>
	public static object? ExecuteScalar(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		ExecuteScalar<object>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query that selects a single value.
	/// </summary>
	/// <typeparam name="T">The type of object to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first column of the first row.</returns>
	public static T? ExecuteScalar<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var command = CreateCommand(connection, sql, parameters, options);
		var value = command.ExecuteScalar();
		return value is null || value is DBNull ? default : (T?) Mapper.Instance.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Returns a value indicating whether an entity with the specified primary key exists.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public static bool Exists<T>(this IDbConnection connection, object id, CommandOptions? options = null) where T: new() {
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), id);
		return ExecuteScalar<bool>(connection, builder.GetExistsCommand<T>(), new(parameter), options);
	}

	/// <summary>
	/// Finds an entity with the specified primary key.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
	public static T? Find<T>(this IDbConnection connection, object id, string[]? columns = null, CommandOptions? options = null) where T: new() {
		var builder = new CommandBuilder(connection);
		var parameter = new Parameter(builder.UsePositionalParameters ? "?1" : builder.GetParameterName("Id"), id);
		return QuerySingleOrDefault<T>(connection, builder.GetFindCommand<T>(columns ?? []), new(parameter), options);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static IEnumerable<ExpandoObject> Query(this IDbConnection connection, string sql, ParameterCollection? parameters = null, QueryOptions? options = null) =>
		Query<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static IEnumerable<T> Query<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, QueryOptions? options = null) where T: new() {
		var records = Mapper.Instance.CreateInstances<T>(ExecuteReader(connection, sql, parameters, options));
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
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static IEnumerable<(T, U)> Query<T, U>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, string splitOn = "Id", QueryOptions? options = null) where T: new() where U: new() {
		var records = Mapper.Instance.CreateInstances<T, U>(ExecuteReader(connection, sql, parameters, options), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QueryFirst(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QueryFirst<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static T QueryFirst<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		using var reader = ExecuteReader(connection, sql, parameters, options);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QueryFirstOrDefault(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QueryFirstOrDefault<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static T? QueryFirstOrDefault<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		using var reader = ExecuteReader(connection, sql, parameters, options);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QuerySingle(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QuerySingle<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static T QuerySingle<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, sql, parameters, options);
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
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QuerySingleOrDefault(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QuerySingleOrDefault<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static T? QuerySingleOrDefault<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, sql, parameters, options);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
