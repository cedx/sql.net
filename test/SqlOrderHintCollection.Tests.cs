namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="SqlOrderHintCollection"/> class.
/// </summary>
[TestClass]
public sealed class SqlOrderHintCollectionTests {

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlOrderHintCollection();
		IsEmpty(collection);

		// It should create a collection from a single order order hint.
		collection = new(new SqlOrderHint("ID", SortOrder.Descending));
		HasCount(1, collection);

		var orderHint = collection.First();
		AreEqual("ID", orderHint.Column);
		AreEqual(SortOrder.Descending, orderHint.SortOrder);

		// It should create a collection from a list of order hints.
		collection = new(new("ID", SortOrder.Descending), new("Name", SortOrder.Ascending));
		HasCount(2, collection);

		orderHint = collection.Last();
		AreEqual("Name", orderHint.Column);
		AreEqual(SortOrder.Ascending, orderHint.SortOrder);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlOrderHintCollection(("Key", SortOrder.Ascending));
		IsTrue(collection.Contains("key"));
		IsTrue(collection.Contains("KEY"));
		IsFalse(collection.Contains("foo"));
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of column names.
		SqlOrderHintCollection collection = new[] { "ID", "Name" };
		CollectionAssert.AreEqual(new[] { "ID", "Name" }, collection.Select(parameter => parameter.Column).ToArray());
		CollectionAssert.AreEqual(new[] { SortOrder.Ascending, SortOrder.Ascending }, collection.Select(parameter => parameter.SortOrder).ToArray());

		// It should create a collection from the specified list of column names.
		collection = new List<string> { "ID", "Name" };
		CollectionAssert.AreEqual(new[] { "ID", "Name" }, collection.Select(parameter => parameter.Column).ToArray());
		CollectionAssert.AreEqual(new[] { SortOrder.Ascending, SortOrder.Ascending }, collection.Select(parameter => parameter.SortOrder).ToArray());

		// It should create a collection from the specified dictionary of column names and sort orders.
		collection = new OrderedDictionary<string, SortOrder>{ ["ID"] = SortOrder.Descending, ["Name"] = SortOrder.Ascending };
		CollectionAssert.AreEqual(new[] { "ID", "Name" }, collection.Select(parameter => parameter.Column).ToArray());
		CollectionAssert.AreEqual(new[] { SortOrder.Descending, SortOrder.Ascending }, collection.Select(parameter => parameter.SortOrder).ToArray());
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));

		// It should return the order hint with the specified column name.
		var orderHint = collection["id"];
		AreEqual("ID", orderHint.Column);
		AreEqual(SortOrder.Descending, orderHint.SortOrder);
		AreEqual(orderHint, collection[0]);

		// It should throw an error if the specified column does not exist.
		Throws<KeyNotFoundException>(() => collection["foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		AreEqual(0, collection.IndexOf("id"));
		AreEqual(1, collection.IndexOf("name"));
		AreEqual(-1, collection.IndexOf("foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the order hint with the specified column name.
		var collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		HasCount(2, collection);
		collection.RemoveAt("name");
		HasCount(1, collection);
		collection.RemoveAt("id");
		IsEmpty(collection);

		// It should throw an error if the specified column does not exist.
		collection = new SqlOrderHintCollection(("ID", SortOrder.Descending), ("Name", SortOrder.Ascending));
		Throws<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
