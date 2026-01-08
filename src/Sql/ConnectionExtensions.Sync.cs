namespace Belin.Sql;

using System.Data;
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
		return value is null || value is DBNull ? default : (T?) mapper.ChangeType(value, typeof(T));
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static IEnumerable<ExpandoObject> Query(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		Query<ExpandoObject>(connection, sql, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static IEnumerable<T> Query<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: class, new() =>
		mapper.CreateInstances<T>(ExecuteReader(connection, sql, parameters, options));

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of the first objects.</typeparam>
	/// <typeparam name="U">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The field from which to split and read the second object.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static IEnumerable<(T, U)> Query<T, U>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, string splitOn = "Id", CommandOptions? options = null) where T: class, new() where U: class, new() =>
		mapper.CreateInstances<T, U>(ExecuteReader(connection, sql, parameters, options), splitOn);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
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
	public static T QueryFirst<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: class, new() {
		using var reader = ExecuteReader(connection, sql, parameters, options);
		return reader.Read() ? mapper.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="sql">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
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
	public static T? QueryFirstOrDefault<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: class, new() {
		using var reader = ExecuteReader(connection, sql, parameters, options);
		return reader.Read() ? mapper.CreateInstance<T>(reader) : default;
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
	public static T QuerySingle<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: class, new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, sql, parameters, options);
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
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
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
	public static T? QuerySingleOrDefault<T>(this IDbConnection connection, string sql, ParameterCollection? parameters = null, CommandOptions? options = null) where T: class, new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, sql, parameters, options);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = mapper.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
