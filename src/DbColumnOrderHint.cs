namespace Belin.Sql;

/// <summary>
/// Defines the sort order for a database column.
/// </summary>
/// <param name="column">The name of the column for which the hint is being provided.</param>
/// <param name="sortOrder">The sort order of the column.</param>
public sealed class DbColumnOrderHint(string column, SortOrder sortOrder = SortOrder.Ascending) {

	/// <summary>
	/// The name of the column for which the hint is being provided.
	/// </summary>
	public string Column { get; } = column;

	/// <summary>
	/// The sort order of the column.
	/// </summary>
	public SortOrder SortOrder { get; set; } = sortOrder;

	/// <summary>
	/// Creates a new order hint from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified tuple.</returns>
	public static implicit operator DbColumnOrderHint((string Column, SortOrder SortOrder) orderHint) => new(orderHint.Column, orderHint.SortOrder);

	/// <summary>
	/// Creates a new order hint from the specified key/value pair.
	/// </summary>
	/// <param name="orderHint">The key/value pair providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified key/value pair.</returns>
	public static implicit operator DbColumnOrderHint(KeyValuePair<string, SortOrder> orderHint) => new(orderHint.Key, orderHint.Value);
}
