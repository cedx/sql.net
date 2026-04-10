namespace Belin.Sql;

using System.Data;
using System.Dynamic;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class ConnectionExtensions {

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static IEnumerable<ExpandoObject> Query(this IDbConnection connection, string text, ParameterCollection? parameters = null, QueryOptions? options = null) =>
		Query<ExpandoObject>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	/// <remarks>Each row can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<IEnumerable<ExpandoObject>> QueryAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, QueryOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryAsync<ExpandoObject>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static IEnumerable<T> Query<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, QueryOptions? options = null) where T: new() {
		var records = Mapper.Instance.CreateInstances<T>(ExecuteReader(connection, text, parameters, options));
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of objects whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, QueryOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		var records = Mapper.Instance.CreateInstances<T>(await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken));
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2)> Query<TItem1, TItem2>(this IDbConnection connection, string text, ParameterCollection? parameters = null, string splitOn = "Id", QueryOptions? options = null) where TItem1: new() where TItem2: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2>(ExecuteReader(connection, text, parameters, options), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object pairs whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The field from which to split and read the next object.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object pairs whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2)>> QueryAsync<TItem1, TItem2>(this IDbConnection connection, string text, ParameterCollection? parameters = null, string splitOn = "Id", QueryOptions? options = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2>(await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2, TItem3)> Query<TItem1, TItem2, TItem3>(this IDbConnection connection, string text, ParameterCollection? parameters = null, (string, string)? splitOn = null, QueryOptions? options = null) where TItem1: new() where TItem2: new() where TItem3: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(ExecuteReader(connection, text, parameters, options), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2, TItem3)>> QueryAsync<TItem1, TItem2, TItem3>(this IDbConnection connection, string text, ParameterCollection? parameters = null, (string, string)? splitOn = null, QueryOptions? options = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3>(await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="options">The query options.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static IEnumerable<(TItem1, TItem2, TItem3, TItem4)> Query<TItem1, TItem2, TItem3, TItem4>(this IDbConnection connection, string text, ParameterCollection? parameters = null, (string, string, string)? splitOn = null, QueryOptions? options = null) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(ExecuteReader(connection, text, parameters, options), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns a sequence of object tuples whose properties correspond to the columns.
	/// </summary>
	/// <typeparam name="TItem1">The type of the first objects.</typeparam>
	/// <typeparam name="TItem2">The type of the second objects.</typeparam>
	/// <typeparam name="TItem3">The type of the third objects.</typeparam>
	/// <typeparam name="TItem4">The type of the fourth objects.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="splitOn">The fields from which to split and read the next objects.</param>
	/// <param name="options">The query options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The sequence of object tuples whose properties correspond to the columns.</returns>
	public static async Task<IEnumerable<(TItem1, TItem2, TItem3, TItem4)>> QueryAsync<TItem1, TItem2, TItem3, TItem4>(this IDbConnection connection, string text, ParameterCollection? parameters = null, (string, string, string)? splitOn = null, QueryOptions? options = null, CancellationToken cancellationToken = default) where TItem1: new() where TItem2: new() where TItem3: new() where TItem4: new() {
		var records = Mapper.Instance.CreateInstances<TItem1, TItem2, TItem3, TItem4>(await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken), splitOn);
		return (options?.Buffered ?? true) ? Enumerable.ToList(records) : records;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QueryFirst(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QueryFirst<ExpandoObject>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QueryFirstAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryFirstAsync<ExpandoObject>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static T QueryFirst<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		using var reader = ExecuteReader(connection, text, parameters, options);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty.</exception>
	public static async Task<T> QueryFirstAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		using var reader = await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : throw new InvalidOperationException("The result set is empty.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QueryFirstOrDefault(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QueryFirstOrDefault<ExpandoObject>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QueryFirstOrDefaultAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QueryFirstOrDefaultAsync<ExpandoObject>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static T? QueryFirstOrDefault<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		using var reader = ExecuteReader(connection, text, parameters, options);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the first row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The first row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QueryFirstOrDefaultAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		using var reader = await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken);
		return reader.Read() ? Mapper.Instance.CreateInstance<T>(reader) : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject QuerySingle(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QuerySingle<ExpandoObject>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject> QuerySingleAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QuerySingleAsync<ExpandoObject>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static T QuerySingle<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, text, parameters, options);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record! : throw new InvalidOperationException("The result set is empty or contains more than one record.");
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row.</returns>
	/// <exception cref="InvalidOperationException">The result set is empty or contains more than one record.</exception>
	public static async Task<T> QuerySingleAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken);
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
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static ExpandoObject? QuerySingleOrDefault(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) =>
		QuerySingleOrDefault<ExpandoObject>(connection, text, parameters, options);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	/// <remarks>The row values can be accessed via <c>dynamic</c> or by casting to a <see cref="IDictionary{string, object?}"/>.</remarks>
	public static async Task<ExpandoObject?> QuerySingleOrDefaultAsync(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) =>
		await QuerySingleOrDefaultAsync<ExpandoObject>(connection, text, parameters, options, cancellationToken);

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static T? QuerySingleOrDefault<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = ExecuteReader(connection, text, parameters, options);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}

	/// <summary>
	/// Executes a parameterized SQL query and returns the single row.
	/// </summary>
	/// <typeparam name="T">The type of objects to return.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="text">The SQL query to be executed.</param>
	/// <param name="parameters">The parameters of the SQL query.</param>
	/// <param name="options">The command options.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The single row, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection connection, string text, ParameterCollection? parameters = null, CommandOptions? options = null, CancellationToken cancellationToken = default) where T: new() {
		T? record = default;
		var rowCount = 0;

		using var reader = await ExecuteReaderAsync(connection, text, parameters, options, cancellationToken);
		while (reader.Read()) {
			if (++rowCount > 1) break;
			record = Mapper.Instance.CreateInstance<T>(reader);
		}

		return rowCount == 1 ? record : default;
	}
}
