namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="ConnectionExtensions"/> class.
/// </summary>
/// <param name="testContext">The test context.</param>
[TestClass]
public sealed class ConnectionExtensionsQueryTests(TestContext testContext) {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = default!;

	[TestInitialize]
	public void TestInitialize() => connection = new SQLiteConnection($"DataSource={Path.Join(AppContext.BaseDirectory, "../res/Database.sqlite")}");

	[TestCleanup]
	public void TestCleanup() => connection.Close();

	[TestMethod]
	public async Task Query() {
		var sql = "SELECT * FROM Characters WHERE Gender = @Gender ORDER BY FullName";
		var parameters = new ParameterCollection("Gender", CharacterGender.Elf.ToString());

		var records = connection.Query<Character>(sql, parameters).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		records = (await connection.QueryAsync<Character>(sql, parameters, cancellationToken: testContext.CancellationToken)).AsList();
		HasCount(3, records);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);
	}

	[TestMethod]
	public async Task QueryFirst() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var parameters = new ParameterCollection("FullName", "Sauron");

		var record = connection.QueryFirst<Character>(sql, parameters);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);

		record = await connection.QueryFirstAsync<Character>(sql, parameters, cancellationToken: testContext.CancellationToken);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);
	}

	[TestMethod]
	public async Task QuerySingle() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var parameters = new ParameterCollection("FullName", "Saruman");

		var record = connection.QuerySingle<Character>(sql, parameters);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);

		record = await connection.QuerySingleAsync<Character>(sql, parameters, cancellationToken: testContext.CancellationToken);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);
	}
}
