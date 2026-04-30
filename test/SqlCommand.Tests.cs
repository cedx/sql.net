namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="SqlCommand"/> class.
/// </summary>
[TestClass]
public sealed class SqlCommandTests {

	[TestMethod]
	public void ImplicitConversion() {
		var sql = "SELECT * FROM Characters";
		AreEqual(sql, ((SqlCommand) sql).Text);
	}
}
