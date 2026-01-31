namespace Belin.Sql.Reflection;

using System.Collections.Frozen;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

/// <summary>
/// Provides information about a database table.
/// </summary>
public sealed class TableInfo {

	/// <summary>
	/// The table columns.
	/// </summary>
	public IReadOnlyDictionary<string, ColumnInfo> Columns { get; }

	/// <summary>
	/// The single identity column, if applicable.
	/// </summary>
	public ColumnInfo? IdentityColumn { get; }

	/// <summary>
	/// The table name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// The table schema.
	/// </summary>
	public string? Schema { get; }

	/// <summary>
	/// The entity type associated with this table.
	/// </summary>
	public Type Type { get; }
	
	/// <summary>
	/// TODO The lock used to safely instanciate the <see cref="ColumnInfo"/> objects.
	/// </summary>
	private readonly Lock lockObject = new();

	/// <summary>
	/// Creates new table information.
	/// </summary>
	/// <param name="type">The type information providing the table metadata.</param>
	public TableInfo(Type type) {
		var table = type.GetCustomAttribute<TableAttribute>();
		var columns = type
			.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
			.Where(property => !property.IsDefined(typeof(NotMappedAttribute)) && ((property.CanRead && property.CanWrite) || property.IsDefined(typeof(ColumnAttribute))))
			.Select(property => { lock (lockObject) { return new ColumnInfo(property); } })
			.ToFrozenDictionary(column => column.Name);

		Columns = columns;
		IdentityColumn = columns.Values.SingleOrDefault(column => column.IsIdentity) ?? (columns.TryGetValue("Id", out var column) ? column : null);
		Name = table?.Name ?? type.Name;
		Schema = table?.Schema;
		Type = type;
	}
}
