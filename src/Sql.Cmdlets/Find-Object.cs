namespace Belin.Sql.Cmdlets;

using System.Data;

/// <summary>
/// Finds the entity with the specified primary key.
/// </summary>
[Cmdlet(VerbsCommon.Find, "Object"), OutputType(typeof(object))]
public class FindObjectCommand: Cmdlet {

	/// <summary>
	/// An array of types representing the number, order, and type of the parameters of the underlying method to invoke.
	/// </summary>
	private static readonly Type[] parameterTypes = [typeof(IDbConnection), typeof(object), typeof(string[]), typeof(CommandOptions)];

	/// <summary>
	/// The type of object to return.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required Type Class { get; set; }

	/// <summary>
	/// The list of columns to select. By default, all columns.
	/// </summary>
	public string[] Columns { get; set; } = [];

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The primary key value.
	/// </summary>
	[Parameter(Mandatory = true, Position = 2)]
	public required object Id { get; set; }

	/// <summary>
	/// The wait time, in seconds, before terminating the attempt to execute the command and generating an error.
	/// </summary>
	[Parameter, ValidateRange(ValidateRangeKind.Positive)]
	public int Timeout { get; set; } = 30;

	/// <summary>
	/// The transaction to use, if any.
	/// </summary>
	[Parameter]
	public IDbTransaction? Transaction { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() {
		var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Find), 1, parameterTypes)!.MakeGenericMethod(Class);
		var arguments = new object[] { Connection, Id, Columns, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
		WriteObject(method.Invoke(null, arguments));
	}
}
