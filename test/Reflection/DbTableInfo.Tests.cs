namespace Belin.Sql.Reflection;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="DbTableInfo"/> class.
/// </summary>
[TestClass]
public sealed class DbTableInfoTests {

	[TestMethod]
	public void Columns() {
		HasCount(0, new DbTableInfo(typeof(DbTableInfo)).Columns);

		var columns = new DbTableInfo(typeof(Character)).Columns;
		HasCount(5, columns);
		CollectionAssert.AreEquivalent(new[] { "firstName", "fullName", "gender", "ID", "lastName" }, columns.Keys.ToArray());
	}

	[TestMethod]
	public void IdentityColumn() {
		IsNull(new DbTableInfo(typeof(DbTableInfo)).IdentityColumn);

		var identityColumn = new DbTableInfo(typeof(Character)).IdentityColumn;
		IsNotNull(identityColumn);
		AreEqual("ID", identityColumn.Name);
	}

	[TestMethod]
	public void Name() {
		// It should return the class name when there is no [Table] attribute.
		AreEqual("DbTableInfo", new DbTableInfo(typeof(DbTableInfo)).Name);

		// It should return the value of the [Table] attribute when it is present.
		AreEqual("Characters", new DbTableInfo(typeof(Character)).Name);
	}

	[TestMethod]
	public void Schema() {
		// It should return `null` when there is no [Table] attribute.
		IsNull(new DbTableInfo(typeof(DbTableInfo)).Schema);

		// It should return the value of the [Table] attribute when it is present.
		AreEqual("main", new DbTableInfo(typeof(Character)).Schema);
	}
}
