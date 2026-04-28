namespace Belin.Sql;

using System.Data;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a parameter of a parameterized SQL statement.
/// </summary>
/// <param name="name">The parameter name.</param>
/// <param name="value">The parameter value.</param>
public sealed class Parameter(string name, object? value = null) {

	/// <summary>
	/// The prefixes used for parameter placeholders.
	/// </summary>
	private static readonly char[] prefixes = ['?', '@', ':', '$'];

	/// <summary>
	/// The database type of this parameter.
	/// </summary>
	public DbType? DbType { get; set; }

	/// <summary>
	/// Value indicating whether this parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.
	/// </summary>
	public ParameterDirection? Direction { get; set; }

	/// <summary>
	/// The parameter name.
	/// </summary>
	public string Name { get; set => field = NormalizeName(value); } = NormalizeName(name);

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
	[NotNull]
	public object? Value { get; set => field = NormalizeValue(value); } = NormalizeValue(value);

	/// <summary>
	/// Creates a new parameter from the specified tuple.
	/// </summary>
	/// <param name="parameter">The tuple providing the parameter properties.</param>
	/// <returns>The parameter corresponding to the specified tuple.</returns>
	public static implicit operator Parameter((string Name, object? Value) parameter) => new(parameter.Name, parameter.Value);

	/// <summary>
	/// Normalizes the specified parameter name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The normalized parameter name.</returns>
	internal static string NormalizeName(string name) => name.Length == 0 ? "?" : (prefixes.Contains(name[0]) ? name : $"@{name}");

	/// <summary>
	/// Normalizes the specified parameter value.
	/// </summary>
	/// <param name="value">The parameter value.</param>
	/// <returns>The normalized parameter value.</returns>
	internal static object NormalizeValue(object? value) => value ?? DBNull.Value;

	/// <summary>
	/// Converts this parameter into an <see cref="IDbDataParameter"/> object.
	/// </summary>
	/// <param name="command">The command to associate with the created parameter.</param>
	/// <returns>The <see cref="IDbDataParameter"/> object corresponding to this parameter.</returns>
	internal IDbDataParameter ToDbParameter(IDbCommand command) {
		var parameter = command.CreateParameter();
		parameter.ParameterName = Name;
		parameter.Value = Value;
		if (DbType is not null) parameter.DbType = DbType.Value;
		if (Direction is not null) parameter.Direction = Direction.Value;
		if (Precision is not null) parameter.Precision = Precision.Value;
		if (Scale is not null) parameter.Scale = Scale.Value;
		if (Size is not null) parameter.Size = Size.Value;
		return parameter;
	}
}
