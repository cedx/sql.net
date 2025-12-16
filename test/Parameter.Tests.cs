namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="Parameter"/> class.
/// </summary>
[TestClass]
public sealed class ParameterTests {

	[TestMethod]
	[DataRow("", "?")]
	[DataRow("?", "?")]
	[DataRow("?1", "?1")]
	[DataRow("foo", "@foo")]
	[DataRow("@bar", "@bar")]
	[DataRow(":baz", ":baz")]
	[DataRow("$qux", "$qux")]
	public void Name(string name, string expected) => AreEqual(expected, new Parameter(name, null).Name);

	[TestMethod]
	public void Value() {
		AreEqual(DBNull.Value, new Parameter("name", null).Value);
		AreEqual(123, new Parameter("name", 123).Value);
		AreEqual(-123.456, new Parameter("name", -123.456).Value);
		AreEqual("", new Parameter("name", "").Value);
		AreEqual("foo", new Parameter("name", "foo").Value);
		AreEqual(DateTime.UnixEpoch, new Parameter("name", DateTime.UnixEpoch).Value);
	}
}
