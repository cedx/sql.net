namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
[TestClass]
public sealed class DbConnectionExtensionsQueryTests(TestContext testContext): DataSourceTests {

	[TestMethod]
	public void Query() {
		var sql = "SELECT * FROM Characters WHERE Gender = @Gender ORDER BY FullName";
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Elf.ToString()));
		var records = connection.Query<Character>(sql, parameters).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);
	}

	[TestMethod]
	public async Task QueryAsync() {
		var sql = "SELECT * FROM Characters WHERE Gender = @Gender ORDER BY FullName";
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Elf.ToString()));
		var records = (await connection.QueryAsync<Character>(sql, parameters, testContext.CancellationToken)).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);
	}

	[TestMethod]
	public void QueryFirst() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var record = connection.QueryFirst<Character>(sql, [("FullName", "Sauron")]);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);
	}

	[TestMethod]
	public async Task QueryFirstAsync() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var record = await connection.QueryFirstAsync<Character>(sql, [("FullName", "Sauron")], testContext.CancellationToken);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);
	}

	[TestMethod]
	public void QuerySingle() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var parameters = new SqlParameterCollection(("FullName", "Saruman"));
		var record = connection.QuerySingle<Character>(sql, parameters);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);
	}

	[TestMethod]
	public async Task QuerySingleAsync() {
		var sql = "SELECT * FROM Characters WHERE FullName = @FullName";
		var parameters = new SqlParameterCollection(("FullName", "Saruman"));
		var record = await connection.QuerySingleAsync<Character>(sql, parameters, testContext.CancellationToken);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);
	}
}
