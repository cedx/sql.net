namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Reflection;

/// <summary>
/// Finds an entity with the specified primary key.
/// </summary>
[Cmdlet(VerbsCommon.Find, "Object"), OutputType(typeof(object))]
public class FindObjectCommand: Cmdlet {

	/// <summary>
	/// The type of object to find.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required Type Class { get; set; }

	/// <summary>
	/// The list of columns to select. By default, all columns.
	/// </summary>
	[Parameter]
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
		try {
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Find))!.MakeGenericMethod(Class);
			var arguments = new object[] { Connection, Id, Columns, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Find-Object:TargetInvocationException", ErrorCategory.InvalidOperation, null));
		}
	}
}
