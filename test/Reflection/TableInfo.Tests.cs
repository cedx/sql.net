namespace Belin.Sql.Reflection;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="TableInfo"/> class.
/// </summary>
[TestClass]
public sealed class TableInfoTests {

	[TestMethod]
	public void Columns() {
		HasCount(0, new TableInfo(typeof(TableInfo)).Columns);

		var columns = new TableInfo(typeof(Character)).Columns;
		HasCount(5, columns);
		CollectionAssert.AreEquivalent(new[] { "firstName", "fullName", "gender", "ID", "lastName" }, columns.Keys.ToArray());
	}

	[TestMethod]
	public void IdentityColumn() {
		IsNull(new TableInfo(typeof(TableInfo)).IdentityColumn);

		var identityColumn = new TableInfo(typeof(Character)).IdentityColumn;
		IsNotNull(identityColumn);
		AreEqual("ID", identityColumn.Name);
	}

	[TestMethod]
	public void Name() {
		// It should return the class name when there is no [Table] attribute.
		AreEqual("TableInfo", new TableInfo(typeof(TableInfo)).Name);

		// It should return the value of the [Table] attribute when it is present.
		AreEqual("Characters", new TableInfo(typeof(Character)).Name);
	}

	[TestMethod]
	public void Schema() {
		// It should return `null` when there is no [Table] attribute.
		IsNull(new TableInfo(typeof(TableInfo)).Schema);

		// It should return the value of the [Table] attribute when it is present.
		AreEqual("main", new TableInfo(typeof(Character)).Schema);
	}
}
