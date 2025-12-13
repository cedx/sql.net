namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

/// <summary>
/// Returns the version of the server associated with the specified connection.
/// </summary>
[Cmdlet(VerbsCommon.Get, "Version"), OutputType(typeof(string), typeof(Version))]
public partial class GetVersionCommand: Cmdlet {

	/// <summary>
	/// Gets the regular expression used to parse the server version.
	/// </summary>
	/// <returns>The regular expression used to parse the server version.</returns>
	[GeneratedRegex(@"^\d+(\.\d+)+")]
	private static partial Regex VersionPattern();

	/// <summary>
	/// The SQL query to be executed.
	/// </summary>
	[Parameter(Position = 1)]
	public string Command { get; set; } = "";

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// Value indicating whether to return a <see cref="Version"/> object.
	/// </summary>
	[Parameter]
	public SwitchParameter PassThru { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		if (string.IsNullOrWhiteSpace(Command)) Command = Connection.GetType().FullName switch {
			"Microsoft.Data.SqlClient.SqlConnection" => "SELECT SERVERPROPERTY('ProductVersion')",
			"Microsoft.Data.Sqlite.SqlConnection" => "SELECT sqlite_version()",
			"MySql.Data.MySqlClient.MySqlConnection" => "SELECT VERSION()",
			"MySqlConnector.MySqlConnection" => "SELECT VERSION()",
			"Npgsql.NpgsqlConnection" => "SHOW server_version",
			"System.Data.SqlClient.SqlConnection" => "SELECT SERVERPROPERTY('ProductVersion')",
			_ => string.Empty
		};

		var serverVersion = Command.Length > 0 ? Connection.ExecuteScalar<string?>(Command) : null;
		if (serverVersion is null && Connection is DbConnection dbConnection) serverVersion = dbConnection.ServerVersion;

		if (!string.IsNullOrWhiteSpace(serverVersion)) {
			var match = VersionPattern().Match(serverVersion);
			if (match.Success) {
				var version = Version.Parse(match.Value);
				WriteObject(PassThru ? version : version.ToString());
				return;
			}
		}

		var exception = new InvalidOperationException("The server version could not be determined.");
		WriteError(new ErrorRecord(exception, "UnknownServerVersion", ErrorCategory.InvalidOperation, null));
		WriteObject(null);
	}
}
