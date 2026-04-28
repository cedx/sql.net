namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="Parameter"/> class.
/// </summary>
[TestClass]
public sealed class ParameterTests {

	[TestMethod]
	public void ImplicitConversion() {
		Parameter parameter = ("", null);
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
		AreEqual(expected, new Parameter(name, null).Name);

	[TestMethod]
	public void Value() {
		AreEqual(DBNull.Value, new Parameter("Name", null).Value);
		AreEqual(DBNull.Value, new Parameter("Name", DBNull.Value).Value);
		AreEqual(123, new Parameter("Name", 123).Value);
		AreEqual(-123.456, new Parameter("Name", -123.456).Value);
		AreEqual("", new Parameter("Name", "").Value);
		AreEqual("Foo", new Parameter("Name", "Foo").Value);
		AreEqual(DateTime.UnixEpoch, new Parameter("Name", DateTime.UnixEpoch).Value);
	}
}
