namespace Belin.Sql;

using System.Data;

/// <summary>
/// Represents a parameter of a parameterized SQL statement.
/// </summary>
/// <param name="name">The parameter name.</param>
/// <param name="value">The parameter value.</param>
/// <param name="dbType">The parameter database type.</param>
/// <param name="size">The parameter maximum size, in bytes.</param>
public sealed class Parameter(string name, object? value, DbType? dbType = null, int? size = null) {

	/// <summary>
	/// The database type of this parameter.
	/// </summary>
	public DbType? DbType { get; set; } = dbType;

	/// <summary>
	/// Value indicating whether this parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
	/// </summary>
	public ParameterDirection? Direction { get; set; }

	/// <summary>
	/// The parameter name.
	/// </summary>
	public string Name { get; set; } = name;

	/// <summary>
	/// Indicates the precision of numeric parameters.
	/// </summary>
	public byte? Precision { get; set; }

	/// <summary>
	/// Indicates the scale of numeric parameters.
	/// </summary>
	public byte? Scale { get; set; }

	/// <summary>
	/// The maximum size of this parameter, in bytes.
	/// </summary>
	public int? Size { get; set; } = size;

	/// <summary>
	/// The parameter value.
	/// </summary>
	public object? Value { get; set; } = value;

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter properties.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	public static implicit operator Parameter((string Name, object? Value) parameter) =>
		new(parameter.Name, parameter.Value);

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter properties.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	public static implicit operator Parameter((string Name, object? Value, DbType DbType) parameter) =>
		new(parameter.Name, parameter.Value, parameter.DbType);

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter properties.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	public static implicit operator Parameter((string Name, object? Value, DbType DbType, int Size) parameter) =>
		new(parameter.Name, parameter.Value, parameter.DbType, parameter.Size);
}
