namespace Belin.Sql;

using System.Data;

/// <summary>
/// Tests the features of the <see cref="ParameterCollection"/> class.
/// </summary>
[TestClass]
public sealed class ParameterCollectionTests {

	[TestMethod]
	public void Constructor() {
		var collection = new ParameterCollection();
		IsEmpty(collection);

		collection = new("?1", 123, DbType.Int32);
		HasCount(1, collection);

		var parameter = collection.First();
		AreEqual("?1", parameter.Name);
		AreEqual(123, parameter.Value);
		AreEqual(DbType.Int32, parameter.DbType);

		collection = new(("?1", 123, DbType.Int32), ("@Key", "Unique", DbType.AnsiString));
		HasCount(2, collection);

		parameter = collection.Last();
		AreEqual("@Key", parameter.Name);
		AreEqual("Unique", parameter.Value);
		AreEqual(DbType.AnsiString, parameter.DbType);
	}

	[TestMethod]
	public void Contains() {
		var collection = new ParameterCollection("@Key");
		IsTrue(collection.Contains("Key"));
		IsTrue(collection.Contains("@Key"));
		IsFalse(collection.Contains("Foo"));
		IsFalse(collection.Contains("@Foo"));
	}

	[TestMethod]
	public void IndexOf() {
		var collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));
		AreEqual(1, collection.IndexOf("Key"));
		AreEqual(1, collection.IndexOf("@Key"));
		AreEqual(-1, collection.IndexOf("Foo"));
		AreEqual(-1, collection.IndexOf("@Foo"));
	}

	[TestMethod]
	public void RemoveAt() {
		var collection = new ParameterCollection(("?1", 123), ("@Key", "Unique"));
		HasCount(2, collection);

		collection.RemoveAt("Key");
		HasCount(1, collection);
		Throws<ArgumentOutOfRangeException>(() => collection.RemoveAt("Foo"));
		collection.RemoveAt("?1");
		IsEmpty(collection);
	}
}
