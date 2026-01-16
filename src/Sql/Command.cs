namespace Belin.Sql;

/// <summary>
/// Represents an SQL statement that is executed while connected to a data source.
/// </summary>
/// <param name="Text">The text of the SQL statement.</param>
/// <param name="Parameters">The parameters of the SQL statement.</param>
public sealed record Command(string Text, ParameterCollection Parameters);
