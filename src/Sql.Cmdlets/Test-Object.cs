namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Reflection;

/// <summary>
/// Checks whether an entity with the specified primary key exists.
/// </summary>
[Cmdlet(VerbsDiagnostic.Test, "Object"), OutputType(typeof(bool))]
public class TestObjectCommand: Cmdlet {

	/// <summary>
	/// An array of types representing the number, order, and type of the parameters of the underlying method to invoke.
	/// </summary>
	private static readonly Type[] parameterTypes = [typeof(IDbConnection), typeof(object), typeof(CommandOptions)];

	/// <summary>
	/// The type of object to check.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required Type Class { get; set; }

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
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Exists))!.MakeGenericMethod(Class);
			var arguments = new object[] { Connection, Id, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Test-Object:TargetInvocationException", ErrorCategory.InvalidOperation, null));
		}
	}
}
