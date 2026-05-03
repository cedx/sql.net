namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static IEnumerable<ExpandoObject> Query(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		Query<ExpandoObject>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<IEnumerable<ExpandoObject>> QueryAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await QueryAsync<ExpandoObject>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static IEnumerable<T> Query<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		var records = SqlMapper.Instance.CreateInstances<T>(reader);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		var records = SqlMapper.Instance.CreateInstances<T>(reader);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2)> Query<TItem1, TItem2>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, string splitOn = "Id") where TItem1: new() where TItem2: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2)>> QueryAsync<TItem1, TItem2>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, string splitOn = "Id", CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2, TItem3)> Query<TItem1, TItem2, TItem3>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, (string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2, TItem3)>> QueryAsync<TItem1, TItem2, TItem3>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, (string, string)? splitOn = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2, TItem3, TItem4)> Query<TItem1, TItem2, TItem3, TItem4>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, (string, string, string)? splitOn = null) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2, TItem3, TItem4)>> QueryAsync<TItem1, TItem2, TItem3, TItem4>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, (string, string, string)? splitOn = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		var records = SqlMapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(reader, splitOn);
		return command.NoEnumerate ? records : Enumerable.ToList(records);
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QueryFirst(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		QueryFirst<ExpandoObject>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QueryFirstAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await QueryFirstAsync<ExpandoObject>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static T QueryFirst<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static async Task<T> QueryFirstAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QueryFirstOrDefault(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		QueryFirstOrDefault<ExpandoObject>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QueryFirstOrDefaultAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await QueryFirstOrDefaultAsync<ExpandoObject>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static T? QueryFirstOrDefault<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();
		return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QueryFirstOrDefaultAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);
		return reader.Read() ? SqlMapper.Instance.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QuerySingle(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		QuerySingle<ExpandoObject>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QuerySingleAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await QuerySingleAsync<ExpandoObject>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static T QuerySingle<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();

		T? record = default;
		var rowCount = 0;
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = SqlMapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

		T? record = default;
		var rowCount = 0;
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = SqlMapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QuerySingleOrDefault(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) =>
		QuerySingleOrDefault<ExpandoObject>(connection, command, parameters);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QuerySingleOrDefaultAsync(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) =>
		await QuerySingleOrDefaultAsync<ExpandoObject>(connection, command, parameters, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static T? QuerySingleOrDefault<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = command.ToDbCommand(connection, parameters);
		using var reader = dbCommand.ExecuteReader();

		T? record = default;
		var rowCount = 0;
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = SqlMapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="command">The command to be executed.</param>
	/// <param name="parameters">The parameters of the SQL statement.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection connection, SqlCommand command, SqlParameterCollection? parameters = null, CancellationToken cancellationToken = default) where T: new() {
		if (connection.State == ConnectionState.Closed) connection.Open();
		using var dbCommand = (DbCommand) command.ToDbCommand(connection, parameters);
		using var reader = await dbCommand.ExecuteReaderAsync(cancellationToken);

		T? record = default;
		var rowCount = 0;
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = SqlMapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
