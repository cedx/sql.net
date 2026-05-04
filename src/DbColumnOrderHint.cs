namespace Belin.Sql;

/// <summary>
/// Defines the sort order for a database column.
/// </summary>
/// <param name="Column">The name of the column for which the hint is being provided.</param>
/// <param name="SortOrder">The sort order of the column.</param>
public sealed record DbColumnOrderHint(string Column, SortOrder SortOrder = SortOrder.Ascending) {

	/// <summary>
	/// Creates a new order hint from the specified key/value pair.
	/// </summary>
	/// <param name="orderHint">The key/value pair providing the column name and its sort order.</param>
	/// <returns>The order hint corresponding to the specified key/value pair.</returns>
	public static implicit operator DbColumnOrderHint(KeyValuePair<string, SortOrder> orderHint) => new(orderHint.Key, orderHint.Value);
}
