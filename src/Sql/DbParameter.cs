namespace Belin.Sql;

using System.Data;

/// <summary>
/// Represents a parameter of a parameterized SQL statement.
/// </summary>
public class DbParameter {

	/// <summary>
	/// The database type of this parameter.
	/// </summary>
	public DbType? DbType { get; set; }

	/// <summary>
	/// Value indicating whether this parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
	/// </summary>
	public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

	/// <summary>
	/// The parameter name.
	/// </summary>
	public required string ParameterName { get; set; }

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
	public int? Size { get; set; }

	/// <summary>
	/// The parameter value.
	/// </summary>
	public object? Value { get; set; }
}
