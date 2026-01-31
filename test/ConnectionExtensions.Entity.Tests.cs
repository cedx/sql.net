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

	//[TestMethod]
	//public async Task ExecuteScalar() {
	//	var sql = "SELECT COUNT(*) FROM Characters WHERE Gender = @Gender";
	//	var parameters = new ParameterCollection("Gender", CharacterGender.Balrog.ToString());
	//	AreEqual(2, connection.ExecuteScalar<int>(sql, parameters));
	//	AreEqual(2, await connection.ExecuteScalarAsync<int>(sql, parameters, cancellationToken: testContext.CancellationToken));
	//}
}
