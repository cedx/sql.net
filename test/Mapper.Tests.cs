namespace Belin.Sql;

/// <summary>
/// Tests the features of the <see cref="Mapper"/> class.
/// </summary>
[TestClass]
public sealed class MapperTests {

	/// <summary>
	/// The test data used by the <see cref="ChangeType"/> method.
	/// </summary>
	private static IEnumerable<object?[]> TestData => [ 
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
		[0, typeof(char?), true, char.MinValue],
		[97, typeof(char), false, 'a'],
		[98, typeof(char?), true, 'b'],
		["a", typeof(char), false, 'a'],
		["b", typeof(char?), true, 'b'],

		[null, typeof(double), false, 0.0],
		[null, typeof(double?), true, null],
		[0, typeof(double), false, 0.0],
		[0, typeof(double?), true, 0.0],
		[123, typeof(double), false, 123.0],
		[-123.456, typeof(double?), true, -123.456],
		["123", typeof(double), false, 123.0],
		["-123.456", typeof(double?), true, -123.456],

		[null, typeof(DateTime), false, DateTime.MinValue],
		[null, typeof(DateTime?), true, null],
		[DateTime.MinValue, typeof(DateTime), false, DateTime.MinValue],
		[DateTime.MaxValue, typeof(DateTime?), true, DateTime.MaxValue],
		[new DateTime(2025, 6, 7, 10, 45, 1), typeof(DateTime), false, new DateTime(2025, 6, 7, 10, 45, 1)],
		[new DateTime(2025, 6, 7, 10, 45, 1), typeof(DateTime?), true, new DateTime(2025, 6, 7, 10, 45, 1)],
		["2025-06-07 10:45:01", typeof(DateTime), false, new DateTime(2025, 6, 7, 10, 45, 1)],
		["2025-06-07T10:45:01", typeof(DateTime?), true, new DateTime(2025, 6, 7, 10, 45, 1)],

		[null, typeof(int), false, 0],
		[null, typeof(int?), true, null],
		[0, typeof(int), false, 0],
		[0, typeof(int?), true, 0],
		[123, typeof(int), false, 123],
		[-123.456, typeof(int?), true, -123],
		["123", typeof(int), false, 123],
		["-123", typeof(int?), true, -123],

		[null, typeof(DayOfWeek), false, DayOfWeek.Sunday],
		[null, typeof(DayOfWeek?), true, null],
		[0, typeof(DayOfWeek), false, DayOfWeek.Sunday],
		[1, typeof(DayOfWeek?), true, DayOfWeek.Monday],
		[5, typeof(DayOfWeek), false, DayOfWeek.Friday],
		[6, typeof(DayOfWeek?), true, DayOfWeek.Saturday],
		["sunday", typeof(DayOfWeek), false, DayOfWeek.Sunday],
		["friday", typeof(DayOfWeek?), true, DayOfWeek.Friday]
	];

	[TestMethod, DynamicData(nameof(TestData))]
	public void ChangeType(object? value, Type conversionType, bool isNullable, object? expected) =>
		AreEqual(expected, new Mapper().ChangeType(value, conversionType, isNullable));
}
