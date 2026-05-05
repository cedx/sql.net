namespace Belin.Sql;

using System.Data;

/// <summary>
/// Tests the features of the <see cref="SqlParameterCollection"/> class.
/// </summary>
[TestClass]
public sealed class SqlParameterCollectionTests {

	[TestMethod]
	public void AddWithValue() {
		var collection = new SqlParameterCollection();
		IsEmpty(collection);

		var parameter = collection.AddWithValue("Name", "Value1");
		HasCount(1, collection);
		AreEqual("@Name", parameter.Name);
		AreEqual("Value1", parameter.Value);

		parameter = collection.AddWithValue("Value2");
		HasCount(2, collection);
		AreEqual("?2", parameter.Name);
		AreEqual("Value2", parameter.Value);
	}

	[TestMethod]
	public void Constructor() {
		// It should create an empty collection by default.
		var collection = new SqlParameterCollection();
		IsEmpty(collection);

		// It should create a collection from a single parameter.
		collection = new(new SqlParameter("?1", 123) { DbType = DbType.Int64 });
		HasCount(1, collection);

		var parameter = collection.First();
		AreEqual("?1", parameter.Name);
		AreEqual(123, parameter.Value);
		AreEqual(DbType.Int64, parameter.DbType);

		// It should create a collection from a list of parameters.
		collection = new(new("?1", 123), new("@Key", "Unique") { DbType = DbType.AnsiString });
		HasCount(2, collection);

		parameter = collection.Last();
		AreEqual("@Key", parameter.Name);
		AreEqual("Unique", parameter.Value);
		AreEqual(DbType.AnsiString, parameter.DbType);
	}

	[TestMethod]
	public void Contains() {
		var collection = new SqlParameterCollection(("@Key", null));
		IsTrue(collection.Contains("Key"));
		IsTrue(collection.Contains("@Key"));
		IsFalse(collection.Contains("Foo"));
		IsFalse(collection.Contains("@Foo"));
	}

	[TestMethod]
	public void ImplicitConversion() {
		// It should create a collection from the specified array of positional parameters.
		SqlParameterCollection collection = new object[] { "foo", "bar" };
		CollectionAssert.AreEqual(new[] { "?1", "?2" }, collection.Select(parameter => parameter.Name).ToArray());
		CollectionAssert.AreEqual(new[] { "foo", "bar" }, collection.Select(parameter => parameter.Value).ToArray());

		// It should create a collection from the specified list of positional parameters.
		collection = new List<object?> { "foo", "bar" };
		CollectionAssert.AreEqual(new[] { "?1", "?2" }, collection.Select(parameter => parameter.Name).ToArray());
		CollectionAssert.AreEqual(new[] { "foo", "bar" }, collection.Select(parameter => parameter.Value).ToArray());

		// It should create a collection from the specified dictionary of named parameters.
		collection = new Dictionary<string, object?> { ["foo"] = "bar", ["baz"] = "qux" };
		CollectionAssert.AreEquivalent(new[] { "@foo", "@baz" }, collection.Select(parameter => parameter.Name).ToArray());
		CollectionAssert.AreEquivalent(new[] { "bar", "qux" }, collection.Select(parameter => parameter.Value).ToArray());
	}

	[TestMethod]
	public void Indexer() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));

		// It should return the parameter with the specified name.
		var parameter = collection["Key"];
		AreEqual("@Key", parameter.Name);
		AreEqual("Unique", parameter.Value);
		AreEqual(parameter, collection[1]);

		// It should throw an error if the specified name does not exist.
		Throws<KeyNotFoundException>(() => collection["@Foo"]);
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		AreEqual(1, collection.IndexOf("Key"));
		AreEqual(1, collection.IndexOf("@Key"));
		AreEqual(-1, collection.IndexOf("Foo"));
		AreEqual(-1, collection.IndexOf("@Foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		// It should remove the parameter with the specified name.
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		HasCount(2, collection);
		collection.RemoveAt("Key");
		HasCount(1, collection);
		collection.RemoveAt("?1");
		IsEmpty(collection);

		// It should throw an error if the specified name does not exist.
		collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		Throws<KeyNotFoundException>(() => collection.RemoveAt("Foo"));
	}
}
