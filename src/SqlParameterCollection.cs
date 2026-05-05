namespace Belin.Sql;

/// <summary>
/// Collects all parameters relevant to a parameterized SQL statement.
/// </summary>
/// <param name="parameters">The collection whose elements are copied to the parameter collection.</param>
public class SqlParameterCollection(params IEnumerable<SqlParameter> parameters): List<SqlParameter>(parameters) {

	/// <summary>
	/// Gets the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The parameter with the specified name.</returns>
	/// <exception cref="KeyNotFoundException">The specified parameter name does not exist.</exception>
	public SqlParameter this[string name] {
		get {
			var normalizedName = SqlParameter.NormalizeName(name);
			return Find(parameter => parameter.Name == normalizedName) ?? throw new KeyNotFoundException(normalizedName);
		}
	}

	/// <summary>
	/// Creates a new parameter collection from the specified array of positional parameters.
	/// </summary>
	/// <param name="parameters">The array whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified array of positional parameters.</returns>
	public static implicit operator SqlParameterCollection(object?[] parameters) => [.. parameters.Select((value, index) =>
		value is SqlParameter parameter ? parameter : new SqlParameter($"?{index + 1}", value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified list of positional parameters.
	/// </summary>
	/// <param name="parameters">The list whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified list of positional parameters.</returns>
	public static implicit operator SqlParameterCollection(List<object?> parameters) => [.. parameters.Select((value, index) =>
		value is SqlParameter parameter ? parameter : new SqlParameter($"?{index + 1}", value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified dictionary of named parameters.
	/// </summary>
	/// <param name="parameters">The dictionary whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified dictionary of named parameters.</returns>
	public static implicit operator SqlParameterCollection(Dictionary<string, object?> parameters) => [.. parameters.Select(entry =>
		entry.Value is SqlParameter parameter ? parameter : new SqlParameter(entry.Key, entry.Value)
	)];

	/// <summary>
	/// Adds a new positional parameter to the end of this collection.
	/// </summary>
	/// <param name="value">The parameter value.</param>
	/// <returns>The newly added parameter.</returns>
	public SqlParameter AddWithValue(object? value) => AddWithValue($"?{Count + 1}", value);

	/// <summary>
	/// Adds a new named parameter to the end of this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="value">The parameter value.</param>
	/// <returns>The newly added parameter.</returns>
	public SqlParameter AddWithValue(string name, object? value) {
		var parameter = new SqlParameter(name, value);
		Add(parameter);
		return parameter;
	}

	/// <summary>
	/// Gets a value indicating whether a parameter in this collection has the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns><see langword="true"/> if this collection contains a parameter with the specified name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string name) {
		var normalizedName = SqlParameter.NormalizeName(name);
		return Exists(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Returns the index of the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The index of the parameter with the specified name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string name) {
		var normalizedName = SqlParameter.NormalizeName(name);
		return FindIndex(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Removes the parameter with the specified name from this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <exception cref="KeyNotFoundException">The specified parameter name does not exist.</exception>
	public void RemoveAt(string name) {
		try { RemoveAt(IndexOf(name)); }
		catch (ArgumentOutOfRangeException e) { throw new KeyNotFoundException(SqlParameter.NormalizeName(name), e); }
	}
}
