namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Query() {
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
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
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
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
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QueryFirst<Character>(sql, [("FullName", "Sauron")]);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);

		// It should throw an error if the query produces no results.
		Throws<InvalidOperationException>(() => connection.QueryFirst(sql, [("FullName", "Cédric")]));
	}

	[TestMethod]
	public async Task QueryFirstAsync() {
		// It should return the first record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QueryFirstAsync<Character>(sql, [("FullName", "Sauron")], testContext.CancellationToken);
		AreEqual("Sauron", record.FirstName);
		AreEqual(CharacterGender.DarkLord, record.Gender);

		// It should throw an error if the query produces no results.
		await ThrowsAsync<InvalidOperationException>(() => connection.QueryFirstAsync(sql, [("FullName", "Cédric")], testContext.CancellationToken));
	}

	[TestMethod]
	public void QuerySingle() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = connection.QuerySingle<Character>(sql, [("FullName", "Saruman")]);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);

		// It should throw an error if the query produces no results.
		Throws<InvalidOperationException>(() => connection.QuerySingle<Character>(sql, [("FullName", "Cédric")]));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		Throws<InvalidOperationException>(() => connection.QuerySingle(sql, [("Gender", CharacterGender.Human.ToString())]));
	}

	[TestMethod]
	public async Task QuerySingleAsync() {
		// It should return the single record produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE fullName = @FullName";
		var record = await connection.QuerySingleAsync<Character>(sql, [("FullName", "Saruman")], testContext.CancellationToken);
		AreEqual("Saruman", record.FirstName);
		AreEqual(CharacterGender.Istari, record.Gender);

		// It should throw an error if the query produces no results.
		await ThrowsAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("FullName", "Cédric")], testContext.CancellationToken));

		// It should throw an error if the query produces more than one result.
		sql = "SELECT * FROM Characters WHERE gender = @Gender";
		await ThrowsAsync<InvalidOperationException>(() => connection.QuerySingleAsync(sql, [("Gender", CharacterGender.Human.ToString())], testContext.CancellationToken));
	}
}
