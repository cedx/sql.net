namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public void Execute() {
		AreEqual(16, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
		AreEqual(2, connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", CharacterGender.Balrog.ToString())]));
		AreEqual(14, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));

		AreEqual(3, connection.Execute("DELETE FROM Characters WHERE gender = @Gender", [("Gender", CharacterGender.Elf.ToString())]));
		AreEqual(11, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
	}

	[TestMethod]
	public async Task ExecuteAsync() {
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Balrog.ToString()));
		AreEqual(16, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
		AreEqual(2, await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken));
		AreEqual(14, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));

		parameters = new SqlParameterCollection(("Gender", CharacterGender.Elf.ToString()));
		AreEqual(3, await connection.ExecuteAsync("DELETE FROM Characters WHERE gender = @Gender", parameters, testContext.CancellationToken));
		AreEqual(11, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public void ExecuteScalar() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		AreEqual(2, connection.ExecuteScalar<int>(sql, [("Gender", CharacterGender.Balrog.ToString())]));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		AreEqual("Characters", connection.ExecuteScalar<string>(sql, [("Name", "Characters"), ("Type", "table")]));
	}

	[TestMethod]
	public async Task ExecuteScalarAsync() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE gender = @Gender";
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Balrog.ToString()));
		AreEqual(2, await connection.ExecuteScalarAsync<int>(sql, parameters, testContext.CancellationToken));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		parameters = new SqlParameterCollection(("Name", "Characters"), ("Type", "table"));
		AreEqual("Characters", await connection.ExecuteScalarAsync<string>(sql, parameters, testContext.CancellationToken));
	}
}
