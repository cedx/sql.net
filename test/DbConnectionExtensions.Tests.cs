namespace Belin.Sql;

using Microsoft.Data.Sqlite;

/// <summary>
/// Provides the base class for tests requiring a data source.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public sealed partial class DbConnectionExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private readonly SqliteConnection connection = new("DataSource=:memory:");

	/// <summary>
	/// Opens a connection to the data source.
	/// </summary>
	[TestInitialize]
	public void TestInitialize() {
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
