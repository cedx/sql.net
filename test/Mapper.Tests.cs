namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="Mapper"/> class.
/// </summary>
[TestClass]
public sealed class MapperTests {

	/// <summary>
	/// The test data used by the <see cref="ChangeType"/> method.
	/// </summary>
	private static IEnumerable<object?[]> ChangeTypeData => [
		[null, typeof(bool), false, false],
		[null, typeof(bool?), true, null],
		[0, typeof(bool), false, false],
		[0, typeof(bool?), true, false],
		[1, typeof(bool), false, true],
		[1, typeof(bool?), true, true],
		["false", typeof(bool), false, false],
		["true", typeof(bool), false, true],

		[null, typeof(char), false, char.MinValue],
		[null, typeof(char?), true, null],
		[0, typeof(char), false, char.MinValue],
		[65_535, typeof(char?), true, char.MaxValue],
		[97, typeof(char), false, 'a'],
		[98, typeof(char?), true, 'b'],
		["a", typeof(char), false, 'a'],
		["b", typeof(char?), true, 'b'],

		[null, typeof(DateTime), false, DateTime.MinValue],
		[null, typeof(DateTime?), true, null],
		[DateTime.MaxValue, typeof(DateTime), false, DateTime.MaxValue],
		[DateTime.UnixEpoch, typeof(DateTime?), true, DateTime.UnixEpoch],
		[new DateTime(2025, 6, 7, 10, 45, 1), typeof(DateTime), false, new DateTime(2025, 6, 7, 10, 45, 1)],
		[new DateTime(2026, 1, 31), typeof(DateTime?), true, new DateTime(2026, 1, 31)],
		["2025-06-07 10:45:01", typeof(DateTime), false, new DateTime(2025, 6, 7, 10, 45, 1)],
		["2025-06-07T10:45:01", typeof(DateTime?), true, new DateTime(2025, 6, 7, 10, 45, 1)],

		[null, typeof(DayOfWeek), false, DayOfWeek.Sunday],
		[null, typeof(DayOfWeek?), true, null],
		[0, typeof(DayOfWeek), false, DayOfWeek.Sunday],
		[1, typeof(DayOfWeek?), true, DayOfWeek.Monday],
		[5, typeof(DayOfWeek), false, DayOfWeek.Friday],
		[6, typeof(DayOfWeek?), true, DayOfWeek.Saturday],
		["sunday", typeof(DayOfWeek), false, DayOfWeek.Sunday],
		["friday", typeof(DayOfWeek?), true, DayOfWeek.Friday],

		[null, typeof(double), false, 0.0],
		[null, typeof(double?), true, null],
		[0, typeof(double), false, 0.0],
		[0, typeof(double?), true, 0.0],
		[123, typeof(double), false, 123.0],
		[-123.456, typeof(double?), true, -123.456],
		["123", typeof(double), false, 123.0],
		["-123.456", typeof(double?), true, -123.456],

		[null, typeof(int), false, 0],
		[null, typeof(int?), true, null],
		[0, typeof(int), false, 0],
		[0, typeof(int?), true, 0],
		[123, typeof(int), false, 123],
		[-123.456, typeof(int?), true, -123],
		["123", typeof(int), false, 123],
		["-123", typeof(int?), true, -123]
	];

	[TestMethod, DynamicData(nameof(ChangeTypeData))]
	public void ChangeType(object? value, Type conversionType, bool isNullable, object? expected) =>
		AreEqual(expected, Mapper.ChangeType(value, conversionType, isNullable));

	[TestMethod]
	public void CreateInstance() {
		var properties = new Dictionary<string, object?> {
			["Class"] = "Bard/minstrel",
			["FirstName"] = "Cédric",
			["Gender"] = CharacterGender.Balrog.ToString(),
			["LastName"] = null
		};

		dynamic instance = Mapper.Instance.CreateInstance(properties);
		AreEqual("Bard/minstrel", instance.Class);
		AreEqual("Cédric", instance.FirstName);
		AreEqual(CharacterGender.Balrog.ToString(), instance.Gender);
		IsNull(instance.LastName);

		var character = Mapper.Instance.CreateInstance<Character>(properties);
		AreEqual("Cédric", character.FirstName);
		AreEqual(CharacterGender.Balrog, character.Gender);
		AreEqual("", character.LastName);
	}

	[TestMethod]
	public void GetTable() {
		var table = Mapper.Instance.GetTable<Character>();
		AreEqual("Characters", table.Name);
		AreEqual("main", table.Schema);
		AreEqual(typeof(Character), table.Type);

		HasCount(5, table.Columns.Keys);
		IsTrue(table.Columns[nameof(Character.FirstName)].CanWrite);
		IsTrue(table.Columns[nameof(Character.FullName)].IsComputed);
		AreEqual(typeof(CharacterGender), table.Columns[nameof(Character.Gender)].Type);
		IsTrue(table.Columns[nameof(Character.Id)].IsIdentity);
		AreEqual(typeof(string), table.Columns[nameof(Character.LastName)].Type);
	}

	[TestMethod]
	public void SplitOn() {
		var record = new List<KeyValuePair<string, object?>> {
			new("Id", 123),
			new("LongLabel", "Hello World!"),
			new("ShortLabel", null),
			new("Id", 456),
			new("FirstName", "Cédric"),
			new("LastName", "Belin"),
			new("RowID", 789)
		};

		var properties = new Dictionary<string, object?> {
			["Id"] = 123,
			["LongLabel"] = "Hello World!",
			["ShortLabel"] = null,
			["FirstName"] = "Cédric",
			["LastName"] = "Belin",
			["RowID"] = 789
		};

		var records = Mapper.SplitOn(record);
		HasCount(1, records);
		CollectionAssert.AreEqual(properties, records[0]);

		records = Mapper.SplitOn(record, "_NonExistent_");
		HasCount(1, records);
		CollectionAssert.AreEqual(properties, records[0]);

		records = Mapper.SplitOn(record, "Id");
		HasCount(2, records);
		CollectionAssert.AreEqual(new Dictionary<string, object?> { ["Id"] = 123, ["LongLabel"] = "Hello World!", ["ShortLabel"] = null }, records[0]);
		CollectionAssert.AreEqual(new Dictionary<string, object?> { ["Id"] = 456, ["FirstName"] = "Cédric", ["LastName"] = "Belin", ["RowID"] = 789 }, records[1]);

		records = Mapper.SplitOn(record, "Id", "RowID", "_Unused_");
		HasCount(3, records);
		CollectionAssert.AreEqual(new Dictionary<string, object?> { ["Id"] = 123, ["LongLabel"] = "Hello World!", ["ShortLabel"] = null }, records[0]);
		CollectionAssert.AreEqual(new Dictionary<string, object?> { ["Id"] = 456, ["FirstName"] = "Cédric", ["LastName"] = "Belin" }, records[1]);
		CollectionAssert.AreEqual(new Dictionary<string, object?> { ["RowID"] = 789 }, records[2]);
	}
}
