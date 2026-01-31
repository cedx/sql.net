namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="ConnectionExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public sealed class ConnectionExtensionsExecuteTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = default!;

	[TestInitialize]
	public void TestInitialize() => connection = That.CreateInMemoryDatabase();

	[TestCleanup]
	public void TestCleanup() => connection.Close();

	[TestMethod]
	public async Task Execute() {
		var parameters = new ParameterCollection("Gender", CharacterGender.Balrog.ToString());
		AreEqual(16, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
		AreEqual(2, connection.Execute("DELETE FROM Characters WHERE Gender = @Gender", parameters));
		AreEqual(14, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));

		parameters = new ParameterCollection("Gender", CharacterGender.Elf.ToString());
		AreEqual(3, await connection.ExecuteAsync("DELETE FROM Characters WHERE Gender = @Gender", parameters, cancellationToken: testContext.CancellationToken));
		AreEqual(11, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public async Task ExecuteScalar() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE Gender = @Gender";
		var parameters = new ParameterCollection("Gender", CharacterGender.Balrog.ToString());
		AreEqual(2, connection.ExecuteScalar<int>(sql, parameters));
		AreEqual(2, await connection.ExecuteScalarAsync<int>(sql, parameters, cancellationToken: testContext.CancellationToken));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = 'table' AND name = @Name";
		parameters = new ParameterCollection("Name", "Characters");
		AreEqual("Characters", connection.ExecuteScalar<string>(sql, parameters));
		AreEqual("Characters", await connection.ExecuteScalarAsync<string>(sql, parameters, cancellationToken: testContext.CancellationToken));
	}
}
