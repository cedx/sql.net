namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Provides the base class for tests requiring a data source.
/// </summary>
public abstract class DataSourceTests {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	protected SQLiteConnection connection = default!;

	/// <summary>
	/// Opens a connection to the data source.
	/// </summary>
	[TestInitialize]
	public void TestInitialize() {
		connection = new SQLiteConnection("DataSource=:memory:");
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "../res/Schema.sql"));
		command.ExecuteNonQuery();
	}

	/// <summary>
	/// Closes the connection to the data source.
	/// </summary>
	[TestCleanup]
	public void TestCleanup() => connection.Close();
}
