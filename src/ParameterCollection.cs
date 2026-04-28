namespace Belin.Sql;

/// <summary>
/// Collects all parameters relevant to a parameterized SQL statement.
/// </summary>
public class ParameterCollection: List<Parameter> {

	/// <summary>
	/// Creates a new parameter collection that has the specified initial capacity.
	/// </summary>
	/// <param name="capacity">The number of parameters that the collection can initially store.</param>
	public ParameterCollection(int capacity): base(capacity) {}

	/// <summary>
	/// Creates a new parameter collection that contains the elements copied from the specified collection.
	/// </summary>
	/// <param name="parameters">The collection whose elements are copied to the parameter collection.</param>
	public ParameterCollection(params IEnumerable<Parameter> parameters): base(parameters) {}

	/// <summary>
	/// Gets the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The parameter with the specified name.</returns>
	/// <exception cref="ArgumentOutOfRangeException">The specified parameter name does not exist.</exception>
	public Parameter this[string name] {
		get {
			var normalizedName = Parameter.NormalizeName(name);
			return Find(parameter => parameter.Name == normalizedName) ?? throw new ArgumentOutOfRangeException(nameof(name));
		}
	}

	/// <summary>
	/// Creates a new parameter collection from the specified list of positional parameters.
	/// </summary>
	/// <param name="parameters">The list whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified list of positional parameters.</returns>
	public static implicit operator ParameterCollection(List<object?> parameters) => [.. parameters.Select((value, index) =>
		value is Parameter parameter ? parameter : new Parameter($"?{index + 1}", value)
	)];

	/// <summary>
	/// Creates a new parameter collection from the specified dictionary of named parameters.
	/// </summary>
	/// <param name="parameters">The dictionary whose elements are copied to the parameter collection.</param>
	/// <returns>The parameter collection corresponding to the specified dictionary of named parameters.</returns>
	public static implicit operator ParameterCollection(Dictionary<string, object?> parameters) => [.. parameters.Select(entry =>
		entry.Value is Parameter parameter ? parameter : new Parameter(entry.Key, entry.Value)
	)];

	/// <summary>
	/// Gets a value indicating whether a parameter in this collection has the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns><see langword="true"/> if this collection contains a parameter with the specified name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string name) {
		var normalizedName = Parameter.NormalizeName(name);
		return Exists(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Returns the index of the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The index of the parameter with the specified name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string name) {
		var normalizedName = Parameter.NormalizeName(name);
		return FindIndex(parameter => parameter.Name == normalizedName);
	}

	/// <summary>
	/// Removes the parameter with the specified name from this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	public void RemoveAt(string name) => RemoveAt(IndexOf(name));
}
