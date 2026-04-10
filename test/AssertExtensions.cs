namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Provides extensions members for test assertions.
/// </summary>
public static class AssertExtensions {
	extension(Assert _) {

		/// <summary>
		/// Creates a new in-memory SQLite database, initialized with a default dataset.
		/// </summary>
		/// <returns>The connection to the newly created in-memory database.</returns>
		public SQLiteConnection CreateInMemoryDatabase() {
			var connection = new SQLiteConnection("DataSource=:memory:");
			connection.Open();

			using var command = connection.CreateCommand();
			command.CommandText = File.ReadAllText(Path.Join(AppContext.BaseDirectory, "../res/Schema.sql"));
			command.ExecuteNonQuery();

			return connection;
		}
	}
}
