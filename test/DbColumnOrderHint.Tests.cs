namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="DbColumnOrderHint"/> class.
/// </summary>
[TestClass]
public sealed class DbColumnOrderHintTests {

	[TestMethod]
	public void ImplicitConversion() {
		// It should create an order hint from the specified tuple.
		DbColumnOrderHint orderHint = ("ID", SortOrder.Descending);
		AreEqual("ID", orderHint.Column);
		AreEqual(SortOrder.Descending, orderHint.SortOrder);

		// It should create an order hint from the specified key/value pair.
		orderHint = new KeyValuePair<string, SortOrder>("Name", SortOrder.Ascending);
		AreEqual("Name", orderHint.Column);
		AreEqual(SortOrder.Ascending, orderHint.SortOrder);
	}
}
