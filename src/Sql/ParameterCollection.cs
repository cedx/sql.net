namespace Belin.Sql;

using System.Collections;
using System.Data;

/// <summary>
/// Collects all parameters relevant to a parameterized SQL statement.
/// </summary>
public class ParameterCollection: List<Parameter> {

	/// <summary>
	/// Creates a new parameter list.
	/// </summary>
	public ParameterCollection(): base() {}

	/// <summary>
	/// Creates a new parameter list that contains the elements copied from the specified collection.
	/// </summary>
	/// <param name="collection">The collection whose elements are copied to the parameter list.</param>
	public ParameterCollection(IEnumerable<Parameter> collection): base(collection) {}

	/// <summary>
	/// Creates a new parameter list that contains the specified parameter.
	/// </summary>
	/// <param name="parameter">The parameter to add to the elements in the parameter list.</param>
	public ParameterCollection(Parameter parameter): this([parameter]) {}

	/// <summary>
	/// Creates a new parameter list that contains the specified parameter.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <param name="value">The parameter value.</param>
	/// <param name="dbType">The parameter database type.</param>
	/// <param name="size">The parameter maximum size, in bytes.</param>
	public ParameterCollection(string name, object? value, DbType? dbType = null, int? size = null): this(new Parameter(name, value, dbType, size)) {}

	/// <summary>
	/// Gets the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The parameter with the specified name.</returns>
	/// <exception cref="ArgumentOutOfRangeException">The specified parameter name does not exist.</exception>
	public Parameter this[string name] => Find(parameter => parameter.Name == name) ?? throw new ArgumentOutOfRangeException(name);

	/// <summary>
	/// Creates a new parameter list from the specified array of positional parameters.
	/// </summary>
	/// <param name="array">The array whose elements are copied to the parameter list.</param>
	/// <returns>The parameter list corresponding to the specified array of positional parameters.</returns>
	public static implicit operator ParameterCollection(object?[] array) => [.. array.Index().Select(entry =>
		entry.Item is Parameter parameter ? parameter : new Parameter($"?{entry.Index + 1}", entry.Item)
	)];

	/// <summary>
	/// Creates a new parameter list from the specified list of positional parameters.
	/// </summary>
	/// <param name="list">The list whose elements are copied to the parameter list.</param>
	/// <returns>The parameter list corresponding to the specified list of positional parameters.</returns>
	public static implicit operator ParameterCollection(ArrayList list) => list.ToArray();

	/// <summary>
	/// Creates a new parameter list from the specified dictionary of named parameters.
	/// </summary>
	/// <param name="dictionary">The dictionary whose elements are copied to the parameter list.</param>
	/// <returns>The parameter list corresponding to the specified dictionary of named parameters.</returns>
	public static implicit operator ParameterCollection(Dictionary<string, object?> dictionary) => [.. dictionary.Select(entry =>
		entry.Value is Parameter parameter ? parameter : new Parameter(entry.Key, entry.Value)
	)];

	/// <summary>
	/// Creates a new parameter list from the specified hash table of named parameters.
	/// </summary>
	/// <param name="hashtable">The hash table whose elements are copied to the parameter list.</param>
	/// <returns>The parameter list corresponding to the specified hash table of named parameters.</returns>
	public static implicit operator ParameterCollection(Hashtable hashtable) =>
		hashtable.Cast<DictionaryEntry>().ToDictionary(entry => entry.Key.ToString() ?? "", entry => entry.Value);

	/// <summary>
	/// Creates a new parameter list from the specified list of positional parameters.
	/// </summary>
	/// <param name="list">The list whose elements are copied to the parameter list.</param>
	/// <returns>The parameter list corresponding to the specified list of positional parameters.</returns>
	public static implicit operator ParameterCollection(List<object?> list) => list.ToArray();

	/// <summary>
	/// Gets a value indicating whether a parameter in this collection has the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns><see langword="true"/> if this collection contains a parameter with the specified name, otherwise <see langword="false"/>.</returns>
	public bool Contains(string name) => Exists(parameter => parameter.Name == name);

	/// <summary>
	/// Returns the index of the parameter with the specified name.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	/// <returns>The index of the parameter with the specified name, or <c>-1</c> if not found.</returns>
	public int IndexOf(string name) => FindIndex(parameter => parameter.Name == name);

	/// <summary>
	/// Removes the parameter with the specified name from this collection.
	/// </summary>
	/// <param name="name">The parameter name.</param>
	public void RemoveAt(string name) => RemoveAt(IndexOf(name));
}
