namespace Belin.Sql;

/// <summary>
/// A collection of hints describing the sort order of columns.
/// </summary>
/// <param name="orderHints">The collection whose elements are copied to the order hint collection.</param>
public class DbColumnOrderHintCollection(params IEnumerable<DbColumnOrderHint> orderHints): List<DbColumnOrderHint>(orderHints) {

	/// <summary>
	/// Gets the order hint with the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The order hint with the specified column name.</returns>
	/// <exception cref="KeyNotFoundException">The specified order hint name does not exist.</exception>
	public DbColumnOrderHint this[string column] =>
		Find(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase)) ?? throw new KeyNotFoundException(column);

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator DbColumnOrderHintCollection(string[] columns) =>
		[.. columns.Select(value => new DbColumnOrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator DbColumnOrderHintCollection(List<string> columns) =>
		[.. columns.Select(value => new DbColumnOrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified dictionary of column names and orders.
	/// </summary>
	/// <param name="orderHints">The dictionary whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified dictionary of column names and orders.</returns>
	public static implicit operator DbColumnOrderHintCollection(OrderedDictionary<string, SortOrder> orderHints) =>
		[.. orderHints.Select(entry => new DbColumnOrderHint(entry.Key, entry.Value))];

	/// <summary>
	/// Gets a value indicating whether an order hint in this collection has the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns><see langword="true"/> if this collection contains an order hint with the specified column name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string column) => Exists(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// Returns the index of the order hint with the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The index of the order hint with the specified column name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string column) => FindIndex(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase));

	/// <summary>
	/// Removes the order hint with the specified column name from this collection.
	/// </summary>
	/// <param name="column">The column name.</param>
	public void RemoveAt(string column) => RemoveAt(IndexOf(column));
}
