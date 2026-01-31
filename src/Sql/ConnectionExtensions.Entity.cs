namespace Belin.Sql;

using Belin.Sql.Reflection;
using System.Data;

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
	public static bool Delete<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetDeleteCommand(instance);
		return Execute(connection, text, parameters, options) > 0;
	}

	/// <summary>
	/// Deletes the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to delete.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if the specified entity has been deleted, otherwise <see langword="false"/>.</returns>
	public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetDeleteCommand(instance);
		return await ExecuteAsync(connection, text, parameters, options) > 0;
	}

	/// <summary>
	/// Checks whether an entity with the specified primary key exists.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="id">The primary key value.</param>
	/// <param name="options">The command options.</param>
	/// <returns><see langword="true"/> if an entity with the specified primary key exists, otherwise <see langword="false"/>.</returns>
	public static bool Exists<T>(this IDbConnection connection, object id, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetExistsCommand<T>(id);
		return ExecuteScalar<bool>(connection, text, parameters, options);
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
		var (text, parameters) = new CommandBuilder(connection).GetExistsCommand<T>(id);
		return await ExecuteScalarAsync<bool>(connection, text, parameters, options);
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
	public static T? Find<T>(this IDbConnection connection, object id, string[]? columns = null, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetFindCommand<T>(id, columns ?? []);
		return QuerySingleOrDefault<T>(connection, text, parameters, options);
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
		var (text, parameters) = new CommandBuilder(connection).GetFindCommand<T>(id, columns ?? []);
		return await QuerySingleOrDefaultAsync<T>(connection, text, parameters, options);
	}

	/// <summary>
	/// Inserts the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to insert.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The generated primary key value.</returns>
	public static long Insert<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetInsertCommand(instance);
		var id = ExecuteScalar<long>(connection, text, parameters, options);
		if (Mapper.Instance.GetTable<T>().IdentityColumn is ColumnInfo column) column.SetValue(instance, Mapper.ChangeType(id, column));
		return id;
	}

	/// <summary>
	/// Inserts the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to insert.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The generated primary key value.</returns>
	public static async Task<long> InsertAsync<T>(this IDbConnection connection, T instance, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetInsertCommand(instance);
		var id = await ExecuteScalarAsync<long>(connection, text, parameters, options);
		if (Mapper.Instance.GetTable<T>().IdentityColumn is ColumnInfo column) column.SetValue(instance, Mapper.ChangeType(id, column));
		return id;
	}

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The number of rows affected.</returns>
	public static int Update<T>(this IDbConnection connection, T instance, string[]? columns = null, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetUpdateCommand(instance, columns ?? []);
		return Execute(connection, text, parameters, options);
	}

	/// <summary>
	/// Updates the specified entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="connection">The connection to the data source.</param>
	/// <param name="instance">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <param name="options">The command options.</param>
	/// <returns>The number of rows affected.</returns>
	public static async Task<int> UpdateAsync<T>(this IDbConnection connection, T instance, string[]? columns = null, CommandOptions? options = null) where T: new() {
		var (text, parameters) = new CommandBuilder(connection).GetUpdateCommand(instance, columns ?? []);
		return await ExecuteAsync(connection, text, parameters, options);
	}
}
