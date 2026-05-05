namespace Belin.Sql;

using System.Data;

/// <summary>
/// Provides extension members for database connections.
/// </summary>
public static partial class DbConnectionExtensions {

	/// <summary>
	/// Deletes the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to delete.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
	public static bool Delete<T>(this IDbConnection connection, T entity, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetDeleteCommand(entity);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return Execute(connection, command, parameters) > 0;
	}

	/// <summary>
	/// Deletes the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to delete.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
	public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entity, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetDeleteCommand(entity);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return await ExecuteAsync(connection, command, parameters, cancellationToken) > 0;
	}

	/// <summary>
	/// Checks whether an entity with the specified primary key exists.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
	public static bool Exists<T>(this IDbConnection connection, object id, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetExistsCommand<T>(id);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return ExecuteScalar<bool>(connection, command, parameters);
	}

	/// <summary>
	/// Checks whether an entity with the specified primary key exists.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
	public static async Task<bool> ExistsAsync<T>(this IDbConnection connection, object id, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetExistsCommand<T>(id);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return await ExecuteScalarAsync<bool>(connection, command, parameters, cancellationToken);
	}

	/// <summary>
	/// Finds an entity with the specified primary key.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
	public static T? Find<T>(this IDbConnection connection, object id, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetFindCommand<T>(id, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return QuerySingleOrDefault<T>(connection, command, parameters);
	}

	/// <summary>
	/// Finds an entity with the specified primary key.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The entity with the specified primary key, or <see langword="null"/> if not found.</returns>
	public static async Task<T?> FindAsync<T>(this IDbConnection connection, object id, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetFindCommand<T>(id, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return await QuerySingleOrDefaultAsync<T>(connection, command, parameters, cancellationToken);
	}

	/// <summary>
	/// Finds all entities of the specified type.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="orderHints">The hints describing the sort order of columns.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns>The list of all entities of the specified type.</returns>
	public static IList<T> FindAll<T>(this IDbConnection connection, SqlOrderHintCollection? orderHints = null, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetFindAllCommand<T>(orderHints, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return Query<T>(connection, command, parameters).AsList();
	}

	/// <summary>
	/// Finds all entities of the specified type.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="orderHints">The hints describing the sort order of columns.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns>The list of all entities of the specified type.</returns>
	public static async Task<IList<T>> FindAllAsync<T>(this IDbConnection connection, SqlOrderHintCollection? orderHints = null, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetFindAllCommand<T>(orderHints, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return (await QueryAsync<T>(connection, command, parameters, cancellationToken)).AsList();
	}

	/// <summary>
	/// Inserts the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to insert.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns>The generated primary key value.</returns>
	public static long Insert<T>(this IDbConnection connection, T entity, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetInsertCommand(entity);
		command.Timeout = timeout;
		command.Transaction = transaction;

		var id = ExecuteScalar<long>(connection, command, parameters);
		if (SqlMapper.Instance.GetTable<T>().IdentityColumn is DbColumnInfo column) column.SetValue(entity, SqlMapper.ChangeType(id, column));
		return id;
	}

	/// <summary>
	/// Inserts the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to insert.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The generated primary key value.</returns>
	public static async Task<long> InsertAsync<T>(this IDbConnection connection, T entity, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetInsertCommand(entity);
		command.Timeout = timeout;
		command.Transaction = transaction;

		var id = await ExecuteScalarAsync<long>(connection, command, parameters, cancellationToken);
		if (SqlMapper.Instance.GetTable<T>().IdentityColumn is DbColumnInfo column) column.SetValue(entity, SqlMapper.ChangeType(id, column));
		return id;
	}

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <returns>The number of rows affected.</returns>
	public static int Update<T>(this IDbConnection connection, T entity, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetUpdateCommand(entity, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return Execute(connection, command, parameters);
	}

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="entity">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <param name="timeout">The wait time, in seconds, before terminating the attempt to execute the command and generating an error.</param>
	/// <param name="transaction">The transaction within which the command executes.</param>
	/// <param name="cancellationToken">The token to cancel the operation.</param>
	/// <returns>The number of rows affected.</returns>
	public static async Task<int> UpdateAsync<T>(this IDbConnection connection, T entity, string[]? columns = null, int timeout = 30, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where T: new() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetUpdateCommand(entity, columns ?? []);
		command.Timeout = timeout;
		command.Transaction = transaction;
		return await ExecuteAsync(connection, command, parameters, cancellationToken);
	}
}
