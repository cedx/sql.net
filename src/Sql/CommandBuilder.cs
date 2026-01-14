namespace Belin.Sql;

using System.Data;
using System.Data.Common;

/// <summary>
/// TODO
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
	/// Creates a new command builder.
	/// </summary>
	/// <param name="connection">The connection to the data source.</param>
	public CommandBuilder(IDbConnection connection) {
		switch (connection.GetType().FullName) {
			case "MySql.Data.MySqlClient.MySqlConnection":
			case "MySqlConnector.MySqlConnection":
				QuotePrefix = QuoteSuffix = "`";
				break;
			case "Microsoft.Data.Sqlite.SqliteConnection":
			case "Npgsql.NpgsqlConnection":
				QuotePrefix = QuoteSuffix = "\"";
				break;
			case "Oracle.ManagedDataAccess.Client.OracleConnection":
				CatalogLocation = CatalogLocation.End;
				CatalogSeparator = "@";
				QuotePrefix = QuoteSuffix = "\"";
				break;
		}
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
