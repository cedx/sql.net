namespace Belin.Sql;

/// <summary>
/// Defines the sort order for a database column.
/// </summary>
/// <param name="Column">The name of the column for which the hint is being provided.</param>
/// <param name="SortOrder">The sort order of the column.</param>
public sealed record DbColumnOrderHint(string Column, SortOrder SortOrder = SortOrder.Ascending) {

	/// <summary>
	/// Creates a new parameter collection from the specified list of positional parameters.
	/// </summary>
	/// <param name="parameters">The list whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified list of positional parameters.</returns>
	public static implicit operator DbColumnOrderHint(KeyValuePair<string, SortOrder> columnOrderHint) => new(columnOrderHint.Key, columnOrderHint.Value);
}
