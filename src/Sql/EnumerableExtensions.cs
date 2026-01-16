namespace Belin.Sql;

/// <summary>
/// Provides extension members for enumerables.
/// </summary>
public static class EnumerableExtensions {
	// TODO (.NET 10) extension(IEnumerable<T> enumerable)

	/// <summary>
	/// Creates a list from the specified enumerable.
	/// If it is already a list, the enumerable is returned without any duplication.
	/// </summary>
	/// <typeparam name="T">The type of enumerable elements.</typeparam>
	/// <returns>A list that contains elements from the input sequence.</returns>
	public static List<T> AsList<T>(this IEnumerable<T> enumerable) => enumerable switch {
		List<T> list => list,
		_ => Enumerable.ToList(enumerable)
	};
}
