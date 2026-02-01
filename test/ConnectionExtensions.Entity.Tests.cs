namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="ConnectionExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public sealed class ConnectionExtensionsEntityTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = default!;

	[TestInitialize]
	public void TestInitialize() => connection = That.CreateInMemoryDatabase();

	[TestCleanup]
	public void TestCleanup() => connection.Close();

	[TestMethod]
	public async Task Delete() {
		var sql = "SELECT * FROM Characters WHERE Id = @Id";

		var record = connection.QuerySingleOrDefault<Character>(sql, new("Id", 1));
		IsNotNull(record);
		IsTrue(connection.Delete(record));
		IsFalse(connection.Delete(record));
		IsNull(connection.QuerySingleOrDefault<Character>(sql, new("Id", 1)));

		record = await connection.QuerySingleOrDefaultAsync<Character>(sql, new("Id", 2), cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsTrue(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsFalse(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsNull(await connection.QuerySingleOrDefaultAsync<Character>(sql, new("Id", 2), cancellationToken: testContext.CancellationToken));
	}
}
