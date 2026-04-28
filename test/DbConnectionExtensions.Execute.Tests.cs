namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbConnectionExtensions"/> class.
/// </summary>
public sealed partial class DbConnectionExtensionsTests {

	[TestMethod]
	public async Task Execute() {
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Balrog.ToString()));
		AreEqual(16, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));
		AreEqual(2, connection.Execute("DELETE FROM Characters WHERE Gender = @Gender", parameters));
		AreEqual(14, connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Characters"));

		parameters = new SqlParameterCollection(("Gender", CharacterGender.Elf.ToString()));
		AreEqual(3, await connection.ExecuteAsync("DELETE FROM Characters WHERE Gender = @Gender", parameters, cancellationToken: testContext.CancellationToken));
		AreEqual(11, await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Characters", cancellationToken: testContext.CancellationToken));
	}

	[TestMethod]
	public async Task ExecuteScalar() {
		var sql = "SELECT COUNT(*) FROM Characters WHERE Gender = @Gender";
		var parameters = new SqlParameterCollection(("Gender", CharacterGender.Balrog.ToString()));
		AreEqual(2, connection.ExecuteScalar<int>(sql, parameters));
		AreEqual(2, await connection.ExecuteScalarAsync<int>(sql, parameters, cancellationToken: testContext.CancellationToken));

		sql = "SELECT tbl_name FROM sqlite_schema WHERE type = @Type AND name = @Name";
		parameters = new SqlParameterCollection([("Name", "Characters"), ("Type", "table")]);
		AreEqual("Characters", connection.ExecuteScalar<string>(sql, parameters));
		AreEqual("Characters", await connection.ExecuteScalarAsync<string>(sql, parameters, cancellationToken: testContext.CancellationToken));
	}
}
