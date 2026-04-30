namespace Belin.Sql;

using System.Data;

/// <summary>
/// Tests the features of the <see cref="SqlParameterCollection"/> class.
/// </summary>
[TestClass]
public sealed class SqlParameterCollectionTests {

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
		// It should create a collection from the specified list of positional parameters.
		SqlParameterCollection collection = new List<object?> { "foo", "bar" };
		HasCount(2, collection);
		AreEqual("?1", collection[0].Name);
		AreEqual("foo", collection[0].Value);
		AreEqual("?2", collection[1].Name);
		AreEqual("bar", collection[1].Value);

		// It should create a collection from the specified dictionary of named parameters.
		collection = new Dictionary<string, object?> { ["foo"] = "bar", ["baz"] = "qux" };
		HasCount(2, collection);
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
		Throws<ArgumentOutOfRangeException>(() => collection["@Foo"]);
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
		var collection = new SqlParameterCollection(("?1", 123), ("@Key", "Unique"));
		HasCount(2, collection);
		collection.RemoveAt("Key");
		HasCount(1, collection);
		Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt("Foo"));
		collection.RemoveAt("?1");
		IsEmpty(collection);
	}
}
