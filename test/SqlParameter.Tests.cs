namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="SqlParameter"/> class.
/// </summary>
[TestClass]
public sealed class SqlParameterTests {

	[TestMethod]
	public void ImplicitConversion() {
		SqlParameter parameter = ("", null);
		AreEqual("?", parameter.Name);
		AreEqual(DBNull.Value, parameter.Value);

		parameter = (":foo", "bar");
		AreEqual(":foo", parameter.Name);
		AreEqual("bar", parameter.Value);

		parameter = ("baz", 123);
		AreEqual("@baz", parameter.Name);
		AreEqual(123, parameter.Value);
	}

	[TestMethod]
	[DataRow("", "?")]
	[DataRow("?", "?")]
	[DataRow("?1", "?1")]
	[DataRow("foo", "@foo")]
	[DataRow("@bar", "@bar")]
	[DataRow(":baz", ":baz")]
	[DataRow("$qux", "$qux")]
	public void Name(string name, string expected) =>
		AreEqual(expected, new SqlParameter(name, null).Name);

	[TestMethod]
	public void Value() {
		AreEqual(DBNull.Value, new SqlParameter("Name", null).Value);
		AreEqual(DBNull.Value, new SqlParameter("Name", DBNull.Value).Value);
		AreEqual(123, new SqlParameter("Name", 123).Value);
		AreEqual(-123.456, new SqlParameter("Name", -123.456).Value);
		AreEqual("", new SqlParameter("Name", "").Value);
		AreEqual("Foo", new SqlParameter("Name", "Foo").Value);
		AreEqual(DateTime.UnixEpoch, new SqlParameter("Name", DateTime.UnixEpoch).Value);
	}
}
