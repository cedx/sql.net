namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Provides extensions members for test assertions.
/// </summary>
public static class AssertExtensions {
	// TODO (.NET 10) extension(Assert _) {

	/// <summary>
	/// Creates a new in-memory SQLite database, initialized with a default dataset.
	/// </summary>
	/// <param name="_">The instance of the assertion functionality.</param>
	/// <returns>The connection to the newly created in-memory database.</returns>
	public static SQLiteConnection CreateInMemoryDatabase(this Assert _) {
		var connection = new SQLiteConnection("DataSource=:memory:");
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "../res/Schema.sql"));
		command.ExecuteNonQuery();

		return connection;
	}
}
