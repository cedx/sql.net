namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Reflection;

/// <summary>
/// Inserts the specified entity.
/// </summary>
[Cmdlet(VerbsData.Publish, "Object"), OutputType(typeof(long))]
public class PublishObjectCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The entity to insert.
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
			var instance = InputObject is PSObject psObject ? psObject.BaseObject : InputObject;
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Insert))!.MakeGenericMethod(instance.GetType());
			var arguments = new object[] { Connection, instance, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Publish-Object:TargetInvocationException", ErrorCategory.InvalidOperation, null));
		}
	}
}
