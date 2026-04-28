namespace Belin.Sql;

using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public sealed partial class DbConnectionExtensionsTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = default!;

	[TestInitialize]
	public void TestInitialize() => connection = That.CreateInMemoryDatabase();

	[TestCleanup]
	public void TestCleanup() => connection.Close();
}
