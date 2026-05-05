namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Delete() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = connection.QuerySingle<Character>(sql, [("Id", 1)]);
		IsTrue(connection.Delete(record));
		IsFalse(connection.Delete(record));
		IsNull(connection.QueryFirstOrDefault<Character>(sql, [("Id", 1)]));
	}

	[TestMethod]
	public async Task DeleteAsync() {
		var sql = "SELECT * FROM Characters WHERE ID = @Id";
		var record = await connection.QuerySingleAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken);
		IsTrue(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsFalse(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsNull(await connection.QueryFirstOrDefaultAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken));
	}

	[TestMethod]
	public void Exists() {
		IsTrue(connection.Exists<Character>(1));
		IsFalse(connection.Exists<Character>(666));
	}

	[TestMethod]
	public async Task ExistsAsync() {
		IsTrue(await connection.ExistsAsync<Character>(1, cancellationToken: testContext.CancellationToken));
		IsFalse(await connection.ExistsAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public void Find() {
		// It should find the record with the specified identifier.
		var record = connection.Find<Character>(2);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = connection.Find<Character>(14);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = connection.Find<Character>(2, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = connection.Find<Character>(14, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		IsNull(connection.Find<Character>(666));
	}

	[TestMethod]
	public async Task FindAsync() {
		// It should find the record with the specified identifier.
		var record = await connection.FindAsync<Character>(2, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		// It should allow selecting a specific set of columns.
		record = await connection.FindAsync<Character>(2, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = await connection.FindAsync<Character>(14, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		// It should return `null` if the record is not found.
		IsNull(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public void FindAll() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = connection.FindAll<Character>();
		HasCount(16, records);
		AreEqual(1, records[0].Id);
		AreEqual("Aragorn", records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = connection.FindAll<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)]);
		HasCount(16, records);
		AreEqual(11, records[0].Id);
		AreEqual("Gothmog", records[0].FullName);
		AreEqual(8, records[15].Id);
		AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = connection.FindAll<Character>(columns: ["gender"]);
		AreEqual(1, records[0].Id);
		AreEqual(CharacterGender.Human, records[0].Gender);
		IsNull(records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual(CharacterGender.DarkLord, records[15].Gender);
		IsNull(records[15].FullName);
	}

	[TestMethod]
	public async Task FindAllAsync() {
		// It should return the complete list of entities, sorted by default according to the identity column.
		var records = await connection.FindAllAsync<Character>(cancellationToken: testContext.CancellationToken);
		HasCount(16, records);
		AreEqual(1, records[0].Id);
		AreEqual("Aragorn", records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual("Sauron", records[15].FullName);

		// It should allow sorting the results by a specific set of columns.
		records = await connection.FindAllAsync<Character>([("gender", SortOrder.Ascending), ("fullName", SortOrder.Descending)], cancellationToken: testContext.CancellationToken);
		HasCount(16, records);
		AreEqual(11, records[0].Id);
		AreEqual("Gothmog", records[0].FullName);
		AreEqual(8, records[15].Id);
		AreEqual("Gandalf", records[15].FullName);

		// It should allow selecting a specific set of columns.
		records = await connection.FindAllAsync<Character>(columns: ["gender"], cancellationToken: testContext.CancellationToken);
		AreEqual(1, records[0].Id);
		AreEqual(CharacterGender.Human, records[0].Gender);
		IsNull(records[0].FullName);
		AreEqual(16, records[15].Id);
		AreEqual(CharacterGender.DarkLord, records[15].Gender);
		IsNull(records[15].FullName);
	}

	[TestMethod]
	public void Insert() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		IsEmpty(connection.Query<Character>(sql));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		AreEqual(0, record.Id);
		IsNull(record.FullName);

		var id = connection.Insert(record);
		IsGreaterThan(16, id);
		AreEqual(id, record.Id);

		var records = connection.Query<Character>(sql).AsList();
		HasCount(1, records);

		var cedric = records[0];
		AreEqual(id, cedric.Id);
		AreEqual("Cédric Belin", cedric.FullName);
		AreEqual(record.Gender, cedric.Gender);
	}

	[TestMethod]
	public async Task InsertAsync() {
		var sql = "SELECT * FROM Characters WHERE firstName = 'Cédric'";
		IsEmpty(await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken));

		var record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
		AreEqual(0, record.Id);
		IsNull(record.FullName);

		var id = await connection.InsertAsync(record, cancellationToken: testContext.CancellationToken);
		IsGreaterThan(16, id);
		AreEqual(id, record.Id);

		var records = (await connection.QueryAsync<Character>(sql, cancellationToken: testContext.CancellationToken)).AsList();
		HasCount(1, records);

		var cedric = records[0];
		AreEqual(id, cedric.Id);
		AreEqual("Cédric Belin", cedric.FullName);
		AreEqual(record.Gender, cedric.Gender);
	}

	[TestMethod]
	public void Update() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = connection.QuerySingle<Character>(sql);
		AreEqual("Sauron", sauron.FullName);
		AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		AreEqual(1, DbConnectionExtensions.Update(connection, sauron));

		sauron = connection.QuerySingle<Character>(sql);
		AreEqual("Sauron The big bad guy", sauron.FullName);
		AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = connection.QuerySingle<Character>(sql);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		AreEqual(1, DbConnectionExtensions.Update(connection, saruman, ["gender"]));

		saruman = connection.QuerySingle<Character>(sql);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}

	[TestMethod]
	public async Task UpdateAsync() {
		// It should update the specified record.
		var sql = "SELECT * FROM Characters WHERE firstName = 'Sauron'";

		var sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Sauron", sauron.FullName);
		AreEqual(CharacterGender.DarkLord, sauron.Gender);

		sauron.LastName = "The big bad guy";
		sauron.Gender = CharacterGender.Istari;
		AreEqual(1, await connection.UpdateAsync(sauron, cancellationToken: testContext.CancellationToken));

		sauron = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Sauron The big bad guy", sauron.FullName);
		AreEqual(CharacterGender.Istari, sauron.Gender);

		// It should allow updating a specific set of columns.
		sql = "SELECT * FROM Characters WHERE firstName = 'Saruman'";

		var saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.Istari, saruman.Gender);

		saruman.LastName = "The traitor";
		saruman.Gender = CharacterGender.DarkLord;
		AreEqual(1, await connection.UpdateAsync(saruman, ["gender"], cancellationToken: testContext.CancellationToken));

		saruman = await connection.QuerySingleAsync<Character>(sql, cancellationToken: testContext.CancellationToken);
		AreEqual("Saruman", saruman.FullName);
		AreEqual(CharacterGender.DarkLord, saruman.Gender);
	}
}
