namespace Belin.Sql;

using System.Data;
using System.Data.Common;

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
	/// Value indicating whether the ADO.NET provider uses positional parameters instead of named parameters for parameterized queries.
	/// </summary>
	public bool UsePositionalParameters { get; set; } = false;

	/// <summary>
	/// Creates a new command builder.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	public CommandBuilder(IDbConnection connection) {
		switch (connection.GetType().FullName) {
			case "MySql.Data.MySqlClient.MySqlConnection":
			case "MySqlConnector.MySqlConnection":
				QuotePrefix = QuoteSuffix = "`";
				break;
			case "FirebirdSql.Data.FirebirdClient.FbConnection":
			case "Microsoft.Data.Sqlite.SqliteConnection":
			case "Npgsql.NpgsqlConnection":
				QuotePrefix = QuoteSuffix = "\"";
				break;
			case "Oracle.ManagedDataAccess.Client.OracleConnection":
				CatalogLocation = CatalogLocation.End;
				CatalogSeparator = "@";
				QuotePrefix = QuoteSuffix = "\"";
				break;
			case "System.Data.OleDb.OleDbConnection":
			case "System.Data.Odbc.OdbcConnection":
				UsePositionalParameters = true;
				break;
		}
	}

	/// <summary>
	/// Gets the automatically generated command required to perform deletions at the data source.
	/// </summary>
	/// <returns>The automatically generated command required to perform deletions.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public string GetDeleteCommand<T>() where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");
		return $"DELETE FROM {QuoteIdentifier(table.Name)} WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : "@Id")}";
	}

	/// <summary>
	/// Gets the automatically generated command required to perform selections at the data source.
	/// </summary>
	/// <param name="columns">The list of columns to select. By default, all columns.</param>
	/// <returns>The automatically generated command required to perform selections.</returns>
	/// <exception cref="InvalidOperationException">The identity column could not be found.</exception>
	public string GetSelectCommand<T>(string[]? columns = null) where T: new() {
		var table = Mapper.Instance.GetTable<T>();
		var identityColumn = table.IdentityColumn ?? throw new InvalidOperationException("The identity column could not be found.");
		var fieldList = columns switch {
			null or [] => "*",
			var names => string.Join(", ", (names.Contains(identityColumn.Name) ? names : [identityColumn.Name, ..names]).Select(QuoteIdentifier))
		};

		return $"SELECT {fieldList} FROM {QuoteIdentifier(table.Name)} WHERE {QuoteIdentifier(identityColumn.Name)} = {(UsePositionalParameters ? "?" : "@Id")}";
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
}
