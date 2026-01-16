namespace Belin.Sql;

using System.Data;
using System.Data.Common;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
/// Automatically generates single-table commands.
/// </summary>
/// <param name="connection">The connection to the data source.</param>
public class CommandBuilder {

	/// <summary>
	/// The position of the catalog name in a qualified table name.
	/// </summary>
	public CatalogLocation CatalogLocation { get; set; } = CatalogLocation.Start;

	/// <summary>
	/// The string used as the catalog separator.
	/// </summary>
	public string CatalogSeparator { get; set; } = ".";

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
	public bool SupportsReturningClause { get; set; } = false;

	/// <summary>
	/// Value indicating whether the ADO.NET provider uses positional parameters.
	/// </summary>
	public bool UsePositionalParameters { get; set; } = false;

	/// <summary>
	/// The SQL function to use when the <c>RETURNING</c> clause is not supported.
	/// </summary>
	private readonly string lastInsertIdFunction = "SCOPE_IDENTITY()";

	/// <summary>
	/// Creates a new command builder.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	public CommandBuilder(IDbConnection connection) {
		switch (connection.GetType().FullName) {
			case "MySql.Data.MySqlClient.MySqlConnection":
			case "MySqlConnector.MySqlConnection":
				QuotePrefix = QuoteSuffix = "`";
				lastInsertIdFunction = "LAST_INSERT_ID()";
				break;
			case "FirebirdSql.Data.FirebirdClient.FbConnection":
			case "Microsoft.Data.Sqlite.SqliteConnection":
			case "Npgsql.NpgsqlConnection":
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
			case "System.Data.OleDb.OleDbConnection":
			case "System.Data.Odbc.OdbcConnection":
				UsePositionalParameters = true;
				break;
		}
	}
	
	/// <summary>
	/// Gets the generated command to delete an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="instance">The entity to delete.</param>
	/// <returns>The generated command to delete an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public Command GetDeleteCommand<T>(T instance) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var parameter = new Parameter(UsePositionalParameters ? "?1" : GetParameterName(identityColumn.Name), identityColumn.GetValue(instance));
		var text = $"""
			DELETE FROM {QuoteIdentifier(table.Name)}
			WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return new(text, new(parameter));
	}

	/// <summary>
	/// Gets the generated command to check the existence of an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="id">The value of the entity's primary key.</param>
	/// <returns>The generated command to check the existence of an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public Command GetExistsCommand<T>(object id) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var parameter = new Parameter(UsePositionalParameters ? "?1" : GetParameterName(identityColumn.Name), id);
		var text = $"""
			SELECT 1
			FROM {QuoteIdentifier(table.Name)}
			WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return new(text, new(parameter));
	}

	/// <summary>
	/// Gets the generated command to find an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="id">The value of the entity's primary key.</param>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <returns>The generated command to find an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public Command GetFindCommand<T>(object id, params string[] columns) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var parameter = new Parameter(UsePositionalParameters ? "?1" : GetParameterName(identityColumn.Name), id);
		var text = $"""
			SELECT {(columns is null || columns.Length == 0 ? "*" : string.Join(", ", columns.Select(QuoteIdentifier)))}
			FROM {QuoteIdentifier(table.Name)}
			WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : parameter.Name)}
			""";

		return new(text, new(parameter));
	}

	/// <summary>
	/// Gets the generated command to insert an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="instance">The entity to insert.</param>
	/// <returns>The generated command to insert an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public Command GetInsertCommand<T>(T instance) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var fields = table.Columns.Values.Where(column => column.CanRead && !column.IsComputed).ToArray();
		var text = $"""
			INSERT INTO {QuoteIdentifier(table.Name)} ({string.Join(", ", fields.Select(field => QuoteIdentifier(field.Name)))})
			VALUES ({string.Join(", ", fields.Select(field => UsePositionalParameters ? "?" : GetParameterName(field.Name)))})
			{(SupportsReturningClause ? $"RETURNING {QuoteIdentifier(identityColumn.Name)}" : $"; SELECT {lastInsertIdFunction};")}
			""";

		return new(text, [
			.. fields.Select((field, index) => (UsePositionalParameters ? $"?{index}" : GetParameterName(field.Name), field.GetValue(instance)))
		]);
	}
	
	/// <summary>
	/// Gets the generated command to update an entity.
	/// </summary>
	/// <typeparam name="T">The entity type.</typeparam>
	/// <param name="instance">The entity to update.</param>
	/// <param name="columns">The list of columns to update. By default, all columns.</param>
	/// <returns>The generated command to update an entity.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public Command GetUpdateCommand<T>(T instance, params string[] columns) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");

		var fields = (columns is null || columns.Length == 0 ? table.Columns.Values : table.Columns.Values.Where(column => columns.Contains(column.Name)))
			.Where(column => column.CanRead && !column.IsComputed)
			.ToArray();

		var text = $"""
			UPDATE {QuoteIdentifier(table.Name)}
			SET {string.Join(", ", fields.Select(field => $"{QuoteIdentifier(field.Name)} = {(UsePositionalParameters ? "?" : GetParameterName(field.Name))}"))}
			WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : GetParameterName(identityColumn.Name))}
			""";

		return new(text, [
			.. fields.Select((field, index) => (UsePositionalParameters ? $"?{index}" : GetParameterName(field.Name), field.GetValue(instance))),
			(GetParameterName(identityColumn.Name), identityColumn.GetValue(instance))
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
	/// Returns the full parameter name corresponding to the specified partial parameter name.
	/// </summary>
	/// <param name="parameterName">The partial name of the parameter.</param>
	/// <returns>The full parameter name corresponding to the specified partial parameter name.</returns>
	internal string GetParameterName(string parameterName) => $"{ParameterPrefix}{parameterName}";
}
