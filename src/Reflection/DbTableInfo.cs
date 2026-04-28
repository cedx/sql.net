namespace Belin.Sql.Reflection;

using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

/// <summary>
/// Provides information about a database table.
/// </summary>
public sealed class DbTableInfo {

	/// <summary>
	/// The table columns.
	/// </summary>
	public IReadOnlyDictionary<string, DbColumnInfo> Columns { get; }

	/// <summary>
	/// The single identity column, if applicable.
	/// </summary>
	public DbColumnInfo? IdentityColumn { get; }

	/// <summary>
	/// The table name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The table schema, if applicable.
	/// </summary>
	public string? Schema { get; }

	/// <summary>
	/// The entity type associated with this table.
	/// </summary>
	public Type Type { get; }

	/// <summary>
	/// Creates new table information.
	/// </summary>
	/// <param name="type">The type information providing the table metadata.</param>
	public DbTableInfo(Type type) {
		var table = type.GetCustomAttribute<TableAttribute>();
		var columns =
			from property in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			where !property.IsDefined(typeof(NotMappedAttribute)) && ((property.CanRead && property.CanWrite) || property.IsDefined(typeof(ColumnAttribute)))
			select new DbColumnInfo(property);

		Columns = columns.ToFrozenDictionary(column => column.Name);
		IdentityColumn = Columns.Values.SingleOrDefault(column => column.IsIdentity) ?? (Columns.TryGetValue("Id", out var column) ? column : null);
		Name = table?.Name ?? type.Name;
		Schema = table?.Schema;
		Type = type;
	}
}
