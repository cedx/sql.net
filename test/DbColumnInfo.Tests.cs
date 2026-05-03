namespace Belin.Sql;

using Belin.Sql.Fixtures;
using DataType = System.Data.DbType;

/// <summary>
/// Tests the features of the <see cref="DbColumnInfo"/> class.
/// </summary>
[TestClass]
public sealed class DbColumnInfoTests {

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanRead(string name) =>
		IsTrue(new DbColumnInfo(typeof(Character).GetProperty(name)!).CanRead);

	[TestMethod]
	[DataRow("FirstName")]
	[DataRow("FullName")]
	[DataRow("Gender")]
	[DataRow("Id")]
	public void CanWrite(string name) =>
		IsTrue(new DbColumnInfo(typeof(Character).GetProperty(name)!).CanWrite);

	[TestMethod]
	[DataRow("FirstName", DataType.String)]
	[DataRow("FullName", DataType.String)]
	[DataRow("Gender", DataType.AnsiString)]
	[DataRow("Id", DataType.Int32)]
	public void DbType(string name, DataType expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).DbType);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", true)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsComputed(string name, bool expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).IsComputed);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", false)]
	[DataRow("Gender", false)]
	[DataRow("Id", true)]
	public void IsIdentity(string name, bool expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).IsIdentity);

	[TestMethod]
	[DataRow("FirstName", false)]
	[DataRow("FullName", true)]
	[DataRow("Gender", false)]
	[DataRow("Id", false)]
	public void IsNullable(string name, bool expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).IsNullable);

	[TestMethod]
	[DataRow("FirstName", "firstName")]
	[DataRow("FullName", "fullName")]
	[DataRow("Gender", "gender")]
	[DataRow("Id", "ID")]
	public void Name(string name, string expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).Name);

	[TestMethod]
	[DataRow("FirstName", typeof(string))]
	[DataRow("FullName", typeof(string))]
	[DataRow("Gender", typeof(CharacterGender))]
	[DataRow("Id", typeof(int))]
	public void PropertyType(string name, Type expected) =>
		AreEqual(expected, new DbColumnInfo(typeof(Character).GetProperty(name)!).PropertyType);

	[TestMethod]
	public void GetValue() {
		var character = new Character { FirstName = "Cédric", LastName = "Belin" };
		AreEqual("Cédric", new DbColumnInfo(typeof(Character).GetProperty("FirstName")!).GetValue(character));
		AreEqual("Belin", new DbColumnInfo(typeof(Character).GetProperty("LastName")!).GetValue(character));
	}

	[TestMethod]
	public void SetValue() {
		var character = new Character { FirstName = "Cédric", LastName = "Belin" };
		new DbColumnInfo(typeof(Character).GetProperty("FirstName")!).SetValue(character, "Anders");
		new DbColumnInfo(typeof(Character).GetProperty("LastName")!).SetValue(character, "Hejlsberg");
		AreEqual("Anders", character.FirstName);
		AreEqual("Hejlsberg", character.LastName);
	}
}
