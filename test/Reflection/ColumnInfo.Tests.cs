namespace Belin.Sql.Reflection;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="ColumnInfo"/> class.
/// </summary>
[TestClass]
public sealed class ColumnInfoTests {

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanRead(string name) =>
		IsTrue(new ColumnInfo(typeof(Character).GetProperty(name)!).CanRead);

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanWrite(string name) =>
		IsTrue(new ColumnInfo(typeof(Character).GetProperty(name)!).CanWrite);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", true)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsComputed(string name, bool expected) =>
		AreEqual(expected, new ColumnInfo(typeof(Character).GetProperty(name)!).IsComputed);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", false)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsIdentity(string name, bool expected) =>
		AreEqual(expected, new ColumnInfo(typeof(Character).GetProperty(name)!).IsIdentity);

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void IsNullable(string name) =>
		IsFalse(new ColumnInfo(typeof(Character).GetProperty(name)!).IsNullable);

	[TestMethod]
	[DataRow("FirstName", "firstName")]
	[DataRow("FullName", "fullName")]
	[DataRow("Gender", "gender")]
	[DataRow("Id", "ID")]
	public void Name(string name, string expected) =>
		AreEqual(expected, new ColumnInfo(typeof(Character).GetProperty(name)!).Name);

	[TestMethod]
	[DataRow("FirstName", typeof(string))]
	[DataRow("FullName", typeof(string))]
	[DataRow("Gender", typeof(CharacterGender))]
	[DataRow("Id", typeof(int))]
	public void Type(string name, Type expected) =>
		AreEqual(expected, new ColumnInfo(typeof(Character).GetProperty(name)!).Type);

	[TestMethod]
	public void GetValue() {
		var character = new Character { FirstName = "Cédric", LastName = "Belin" };
		AreEqual("Cédric", new ColumnInfo(typeof(Character).GetProperty("FirstName")!).GetValue(character));
		AreEqual("Belin", new ColumnInfo(typeof(Character).GetProperty("LastName")!).GetValue(character));
	}

	[TestMethod]
	public void SetValue() {
		var character = new Character { FirstName = "Cédric", LastName = "Belin" };
		new ColumnInfo(typeof(Character).GetProperty("FirstName")!).SetValue(character, "Anders");
		new ColumnInfo(typeof(Character).GetProperty("LastName")!).SetValue(character, "Hejlsberg");
		AreEqual("Anders", character.FirstName);
		AreEqual("Hejlsberg", character.LastName);
	}
}
