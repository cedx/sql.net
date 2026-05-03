namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// Automatically generates single-table commands.
/// </summary>
public class SqlCommandBuilder {

	/// <summary>
	/// The position of the catalog name in a qualified table name.
	/// </summary>
	public CatalogLocation CatalogLocation { get; set; } = CatalogLocation.Start;

	/// <summary>
	/// The string used as the catalog separator.
	/// </summary>
	public string CatalogSeparator { get; set; } = ".";

	/// <summary>
	/// The SQL function to use when the <c>RETURNING</c> clause is not supported.
	/// </summary>
	public string LastInsertIdFunction { get; set; } = "SCOPE_IDENTITY()";

	/// <summary>
	/// The beginning string to use for naming parameters.
	/// </summary>
	public string ParameterPrefix { get; set; } = "@";

	/// <summary>
	/// The beginning string to use when specifying database objects.
	/// </summary>
	public string QuotePrefix { get; set; } = "[";

	/// <summary>
	/// The ending string to use when specifying database objects.
	/// </summary>
	public string QuoteSuffix { get; set; } = "]";

	/// <summary>
	/// The string used as the schema separator.
	/// </summary>
	public string SchemaSeparator { get; set; } = ".";

	/// <summary>
	/// Value indicating whether the ADO.NET provider supports the <c>RETURNING</c> clause.
	/// </summary>
	public bool SupportsReturningClause { get; set; }

	/// <summary>
	/// Value indicating whether the ADO.NET provider uses positional parameters.
	/// </summary>
	public bool UsePositionalParameters { get; set; }

	/// <summary>
	/// Creates a new command builder.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	public SqlCommandBuilder(IDbConnection connection) {
		switch (connection.GetType().FullName) {
			case "MySql.Data.MySqlClient.MySqlConnection":
			case "MySqlConnector.MySqlConnection":
				QuotePrefix = QuoteSuffix = "`";
				LastInsertIdFunction = "LAST_INSERT_ID()";
				break;
			case "FirebirdSql.Data.FirebirdClient.FbConnection":
			case "Microsoft.Data.Sqlite.SqliteConnection":
			case "Npgsql.NpgsqlConnection":
			case "System.Data.SQLite.SQLiteConnection":
				QuotePrefix = QuoteSuffix = "\"";
				SupportsReturningClause = true;
				break;
			case "Oracle.ManagedDataAccess.Client.OracleConnection":
				CatalogLocation = CatalogLocation.End;
				CatalogSeparator = "@";
				ParameterPrefix = ":";
				QuotePrefix = QuoteSuffix = "\"";
				SupportsReturningClause = true;
				break;
			case "System.Data.Odbc.OdbcConnection":
			case "System.Data.OleDb.OleDbConnection":
				UsePositionalParameters = true;
				break;
		}
	}

	/// <summary>
	/// Gets the generated command to delete an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="entity">The entity to delete.</param>
	/// <returns>The generated command to delete an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public (SqlCommand Command, SqlParameterCollection Parameters) GetDeleteCommand<T>(T entity) where T: new() {
		var table = SqlMapper.Instance.GetTable<T>();
		var idColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var parameter = new SqlParameter(UsePositionalParameters ? "?1" : GetParameterName(idColumn), GetParameterValue(idColumn, entity));
		var text = $"""
			DELETE FROM {GetTableName(table)}
			WHERE {QuoteIdentifier(idColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return (text, [parameter]);
	}

	/// <summary>
	/// Gets the generated command to check the existence of an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="id">The value of the entity's primary key.</param>
	/// <returns>The generated command to check the existence of an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public (SqlCommand Command, SqlParameterCollection Parameters) GetExistsCommand<T>(object id) where T: new() {
		var table = SqlMapper.Instance.GetTable<T>();
		var idColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var parameter = new SqlParameter(UsePositionalParameters ? "?1" : GetParameterName(idColumn), id);
		var text = $"""
			SELECT 1
			FROM {GetTableName(table)}
			WHERE {QuoteIdentifier(idColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return (text, [parameter]);
	}

	/// <summary>
	/// Gets the generated command to find an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="id">The value of the entity's primary key.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <returns>The generated command to find an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public (SqlCommand Command, SqlParameterCollection Parameters) GetFindCommand<T>(object id, params string[] columns) where T: new() {
		var table = SqlMapper.Instance.GetTable<T>();
		var idColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var fields = (columns.Length == 0 ? table.Columns.Values : table.Columns.Values.Where(column => columns.Contains(column.Name)))
			.Where(column => column.CanWrite)
			.Select(column => column.Name)
			.ToList();

		if (!fields.Contains(idColumn.Name)) fields.Add(idColumn.Name);

		var parameter = new SqlParameter(UsePositionalParameters ? "?1" : GetParameterName(idColumn), id);
		var text = $"""
			SELECT {string.Join(", ", fields.Select(QuoteIdentifier))}
			FROM {GetTableName(table)}
			WHERE {QuoteIdentifier(idColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return (text, [parameter]);
	}

	/// <summary>
	/// Gets the generated command to insert an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="entity">The entity to insert.</param>
	/// <returns>The generated command to insert an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public (SqlCommand Command, SqlParameterCollection Parameters) GetInsertCommand<T>(T entity) where T: new() {
		var table = SqlMapper.Instance.GetTable<T>();
		var idColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var fields = table.Columns.Values.Where(column => column.CanRead && !column.IsComputed).ToArray();
		var text = $"""
			INSERT INTO {GetTableName(table)} ({string.Join(", ", fields.Select(field => QuoteIdentifier(field.Name)))})
			VALUES ({string.Join(", ", fields.Select(field => UsePositionalParameters ? "?" : GetParameterName(field)))})
			{(SupportsReturningClause ? $"RETURNING {QuoteIdentifier(idColumn.Name)}" : $"; SELECT {LastInsertIdFunction};")}
			""";

		return (text, [.. fields.Select((field, index) => (UsePositionalParameters ? $"?{index + 1}" : GetParameterName(field), GetParameterValue(field, entity)))]);
	}

	/// <summary>
	/// Gets the generated command to update an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="entity">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <returns>The generated command to update an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public (SqlCommand Command, SqlParameterCollection Parameters) GetUpdateCommand<T>(T entity, params string[] columns) where T: new() {
		var table = SqlMapper.Instance.GetTable<T>();
		var idColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var fields = (columns.Length == 0 ? table.Columns.Values : table.Columns.Values.Where(column => columns.Contains(column.Name)))
			.Where(column => column.CanRead && !column.IsComputed)
			.ToArray();

		var text = $"""
			UPDATE {GetTableName(table)}
			SET {string.Join(", ", fields.Select(field => $"{QuoteIdentifier(field.Name)} = {(UsePositionalParameters ? "?" : GetParameterName(field))}"))}
			WHERE {QuoteIdentifier(idColumn.Name)} = {(UsePositionalParameters ? "?" : GetParameterName(idColumn))}
			""";

		return (text, [
			.. fields.Select((field, index) => (UsePositionalParameters ? $"?{index + 1}" : GetParameterName(field), GetParameterValue(field, entity))),
			(UsePositionalParameters ? $"?{fields.Length + 1}" : GetParameterName(idColumn), GetParameterValue(idColumn, entity))
		]);
	}

	/// <summary>
	/// Given an unquoted identifier, returns the correct quoted form of that identifier.
	/// </summary>
	/// <param name="unquotedIdentifier">The original unquoted identifier.</param>
	/// <returns>The quoted version of the identifier.</returns>
	public string QuoteIdentifier(string unquotedIdentifier) =>
		$"{QuotePrefix}{unquotedIdentifier.Replace(QuoteSuffix, QuoteSuffix + QuoteSuffix)}{QuoteSuffix}";

	/// <summary>
	/// Given a quoted identifier, returns the correct unquoted form of that identifier.
	/// </summary>
	/// <param name="quotedIdentifier">The original quoted identifier.</param>
	/// <returns>The unquoted version of the identifier.</returns>
	public string UnquoteIdentifier(string quotedIdentifier) {
		if (quotedIdentifier.StartsWith(QuotePrefix, StringComparison.Ordinal)) quotedIdentifier = quotedIdentifier[QuotePrefix.Length..];
		if (quotedIdentifier.EndsWith(QuoteSuffix, StringComparison.Ordinal)) quotedIdentifier = quotedIdentifier[..^QuoteSuffix.Length];
		return quotedIdentifier.Replace(QuoteSuffix + QuoteSuffix, QuoteSuffix);
	}

	/// <summary>
	/// Returns the parameter name corresponding to the specified column.
	/// </summary>
	/// <param name="column">The column providing the parameter name.</param>
	/// <returns>The parameter name corresponding to the specified column.</returns>
	private string GetParameterName(DbColumnInfo column) => $"{ParameterPrefix}{column.Name}";

	/// <summary>
	/// Returns the parameter value corresponding to the specified column.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="column">The column providing the parameter data type.</param>
	/// <param name="entity">The entity providing the parameter value.</param>
	/// <returns>The parameter value corresponding to the specified column.</returns>
	private object? GetParameterValue<T>(DbColumnInfo column, T entity) where T: new() {
		var value = column.GetValue(entity);
		return column.DbType switch {
			DbType.AnsiString or DbType.AnsiStringFixedLength or DbType.String or DbType.StringFixedLength when column.PropertyType.IsEnum => value?.ToString(),
			_ => value
		};
	}

	/// <summary>
	/// Returns the fully qualified name corresponding to the specified table.
	/// </summary>
	/// <param name="table">The table.</param>
	/// <returns>The fully qualified name corresponding to the specified table.</returns>
	private string GetTableName(DbTableInfo table) =>
		string.IsNullOrEmpty(table.Schema) ? QuoteIdentifier(table.Name) : $"{QuoteIdentifier(table.Schema)}{SchemaSeparator}{QuoteIdentifier(table.Name)}";
}
