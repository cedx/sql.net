namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Reflection;

/// <summary>
/// Updates the specified entity.
/// </summary>
[Cmdlet(VerbsData.Update, "Object"), OutputType(typeof(int))]
public class UpdateObjectCommand: Cmdlet {

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
	/// The entity to update.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1, ValueFromPipeline = true)]
	public required object InputObject { get; set; }

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
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Update))!.MakeGenericMethod(InputObject.GetType());
			var arguments = new object[] { Connection, InputObject, Columns, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Update-Object:TargetInvocationException", ErrorCategory.InvalidOperation, null));
		}
	}
}
