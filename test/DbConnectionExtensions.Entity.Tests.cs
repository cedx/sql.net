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

		// It should return `null` if the record is not found.
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
	// 	var sql = "SELECT 1 FROM Characters WHERE fullName = @FullName";

	// 	var record = new Character { FirstName = "Cédric", Gender = CharacterGender.DarkLord };
	// 	AreEqual(0, record.Id);
	// 	IsFalse(connection.ExecuteScalar<bool>(sql, new("FullName", record.FullName)));

	// 	var id = connection.Insert(record);
	// 	AreEqual(id, record.Id);
	// 	IsTrue(connection.ExecuteScalar<bool>(sql, new("FullName", record.FullName)));

	// 	record = new Character { FirstName = "Cédric", LastName = "Belin", Gender = CharacterGender.Istari };
	// 	AreEqual(0, record.Id);
	// 	IsFalse(await connection.ExecuteScalarAsync<bool>(sql, new("FullName", record.FullName), testContext.CancellationToken));

	// 	id = await connection.InsertAsync(record, testContext.CancellationToken);
	// 	AreEqual(id, record.Id);
	// 	IsTrue(await connection.ExecuteScalarAsync<bool>(sql, new("FullName", record.FullName), testContext.CancellationToken));
	// }
}
