namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Dynamic;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Query() {
		// It should return the records produced by the SQL query.
		var sql = "SELECT * FROM Characters WHERE gender = @Gender ORDER BY fullName";
		var records = connection.Query<Character>(sql, [("Gender", CharacterGender.Elf.ToString())]).AsList();
		HasCount(3, records);

		var elrond = records[0];
		AreEqual("Elrond", elrond.FullName);
		AreEqual(CharacterGender.Elf, elrond.Gender);

		var galadriel = records[1];
		AreEqual("Galadriel", galadriel.FullName);
		AreEqual(CharacterGender.Elf, galadriel.Gender);

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = connection.Query<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")]).AsList();
		HasCount(1, objects);

		dynamic left = objects[0].Item1;
		AreEqual(6, left.ID);
		AreEqual("Frodo", left.firstName);
		AreEqual("Baggins", left.lastName);
		IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		AreEqual(6, right.ID);
		AreEqual("Frodo Baggins", right.fullName);
		AreEqual("Hobbit", right.gender);
		IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
	}

	[TestMethod]
	public async Task QueryAsync() {
		// It should return the records produced by the SQL query.
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

		// It should allow the data rows to be split into distinct objects.
		sql = "SELECT ID, firstName, lastName, ID, fullName, gender FROM Characters WHERE firstName = @FirstName";
		var objects = (await connection.QueryAsync<ExpandoObject, ExpandoObject>(sql, [("FirstName", "Frodo")], "id", testContext.CancellationToken)).AsList();
		HasCount(1, objects);

		dynamic left = objects[0].Item1;
		AreEqual(6, left.ID);
		AreEqual("Frodo", left.firstName);
		AreEqual("Baggins", left.lastName);
		IsFalse(((IDictionary<string, object?>) left).ContainsKey("fullName"));

		dynamic right = objects[0].Item2;
		AreEqual(6, right.ID);
		AreEqual("Frodo Baggins", right.fullName);
		AreEqual("Hobbit", right.gender);
		IsFalse(((IDictionary<string, object?>) right).ContainsKey("firstName"));
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
