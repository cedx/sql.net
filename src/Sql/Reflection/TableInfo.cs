namespace Belin.Sql.Reflection;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

/// <summary>
/// Provides information about a database table.
/// </summary>
/// <param name="type">The type information providing the table metadata.</param>
public sealed class TableInfo(Type type) {

	/// <summary>
	/// The table columns.
	/// </summary>
	public IReadOnlyDictionary<string, ColumnInfo> Columns { get; } = type
		.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
		.Where(property => !property.IsDefined(typeof(NotMappedAttribute)) && ((property.CanRead && property.CanWrite) || property.IsDefined(typeof(ColumnAttribute))))
		.Select(property => new ColumnInfo(property))
		.ToDictionary(column => column.Name);

	/// <summary>
	/// The single identity column, if applicable.
	/// </summary>
	public ColumnInfo? IdentityColumn => Columns.Values
		.SingleOrDefault(column => column.IsIdentity) ?? (Columns.TryGetValue("Id", out var column) ? column : null);

	/// <summary>
	/// The table name.
	/// </summary>
	public string Name { get; } = type.GetCustomAttribute<TableAttribute>()?.Name ?? type.Name;
}
