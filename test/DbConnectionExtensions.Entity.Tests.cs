namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
[TestClass]
public sealed class DbConnectionExtensionsEntityTests(TestContext testContext): SqliteTests {

	[TestMethod]
	public void Delete() {
		var sql = "SELECT * FROM Characters WHERE Id = @Id";
		var record = connection.QuerySingleOrDefault<Character>(sql, [("Id", 1)]);

		IsNotNull(record);
		IsTrue(connection.Delete(record));
		IsFalse(connection.Delete(record));
		IsNull(connection.QuerySingleOrDefault<Character>(sql, [("Id", 1)]));
	}

	[TestMethod]
	public async Task DeleteAsync() {
		var sql = "SELECT * FROM Characters WHERE Id = @Id";
		var record = await connection.QuerySingleOrDefaultAsync<Character>(sql, [("Id", 2)], cancellationToken: testContext.CancellationToken);

		IsNotNull(record);
		IsTrue(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsFalse(await connection.DeleteAsync(record, cancellationToken: testContext.CancellationToken));
		IsNull(await connection.QuerySingleOrDefaultAsync<Character>(sql, [("Id", 2)], testContext.CancellationToken));
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
		var record = connection.Find<Character>(2);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = connection.Find<Character>(2, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = connection.Find<Character>(14);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		record = connection.Find<Character>(14, ["gender"]);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		IsNull(connection.Find<Character>(666));
	}

	[TestMethod]
	public async Task FindAsync() {
		var record = await connection.FindAsync<Character>(2, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(2, record.Id);
		AreEqual("Balin", record.FullName);

		record = await connection.FindAsync<Character>(2, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Dwarf, record.Gender);

		record = await connection.FindAsync<Character>(14, cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		AreEqual(14, record.Id);
		AreEqual("Sam Gamgee", record.FullName);

		record = await connection.FindAsync<Character>(14, ["gender"], cancellationToken: testContext.CancellationToken);
		IsNotNull(record);
		IsNull(record.FullName);
		AreEqual(CharacterGender.Hobbit, record.Gender);

		IsNull(await connection.FindAsync<Character>(666, cancellationToken: testContext.CancellationToken));
	}

	// [TestMethod]
	// TODO public async Task Insert() {
	// 	var sql = "SELECT 1 FROM Characters WHERE FullName = @FullName";

	// 	var record = new Character { FirstName = "Cédric", Gender = CharacterGender.DarkLord };
	// 	AreEqual(0, record.Id);
	// 	IsFalse(connection.ExecuteScalar<bool>(sql, new("FullName", record.FullName)));

	// 	var id = connection.Insert(record);
	// 	AreEqual(id, record.Id);
	// 	IsTrue(connection.ExecuteScalar<bool>(sql, new("FullName", record.FullName)));

	// 	record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
	// 	AreEqual(0, record.Id);
	// 	IsFalse(await connection.ExecuteScalarAsync<bool>(sql, new("FullName", record.FullName), cancellationToken: testContext.CancellationToken));

	// 	id = await connection.InsertAsync(record, cancellationToken: testContext.CancellationToken);
	// 	AreEqual(id, record.Id);
	// 	IsTrue(await connection.ExecuteScalarAsync<bool>(sql, new("FullName", record.FullName), cancellationToken: testContext.CancellationToken));
	// }
}
