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

	[TestMethod]
	public async Task Exists() {
		IsTrue(connection.Exists<Character>(1));
		IsTrue(await connection.ExistsAsync<Character>(1, cancellationToken: testContext.CancellationToken));
		IsFalse(connection.Exists<Character>(666));
		IsFalse(await connection.ExistsAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public async Task Find() {
		var record = connection.Find<Character>(2);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = connection.Find<Character>(2, ["gender"])!;
		AreEqual("", record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		record = connection.Find<Character>(14, ["gender"])!;
		AreEqual("", record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		IsNull(connection.Find<Character>(666));
		IsNull(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}
}
