namespace Belin.Sql;

/// <summary>
/// A collection of hints describing the sort order of columns.
/// </summary>
/// <param name="orderHints">The collection whose elements are copied to the order hint collection.</param>
public class SqlOrderHintCollection(params IEnumerable<SqlOrderHint> orderHints): List<SqlOrderHint>(orderHints) {

	/// <summary>
	/// Gets the order hint with the specified column name.
	/// </summary>
	/// <param name="column">The column name.</param>
	/// <returns>The order hint with the specified column name.</returns>
	/// <exception cref="KeyNotFoundException">The specified column name does not exist.</exception>
	public SqlOrderHint this[string column] =>
		Find(orderHint => orderHint.Column.Equals(column, StringComparison.OrdinalIgnoreCase)) ?? throw new KeyNotFoundException(column);

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator SqlOrderHintCollection(string[] columns) =>
		[.. columns.Select(value => new SqlOrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified array of column names.
	/// </summary>
	/// <param name="columns">The array whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified array of column names.</returns>
	public static implicit operator SqlOrderHintCollection(List<string> columns) =>
		[.. columns.Select(value => new SqlOrderHint(value, SortOrder.Ascending))];

	/// <summary>
	/// Creates a new order hint collection from the specified dictionary of column names and orders.
	/// </summary>
	/// <param name="orderHints">The dictionary whose elements are copied to the order hint collection.</param>
	/// <returns>The order hint collection corresponding to the specified dictionary of column names and orders.</returns>
	public static implicit operator SqlOrderHintCollection(OrderedDictionary<string, SortOrder> orderHints) =>
		[.. orderHints.Select(entry => new SqlOrderHint(entry.Key, entry.Value))];

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
	/// <exception cref="KeyNotFoundException">The specified column name does not exist.</exception>
	public void RemoveAt(string column) {
		try { RemoveAt(IndexOf(column)); }
		catch (ArgumentOutOfRangeException e) { throw new KeyNotFoundException(column, e); }
	}
}
