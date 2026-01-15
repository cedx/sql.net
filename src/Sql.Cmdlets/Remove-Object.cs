namespace Belin.Sql.Cmdlets;

using System.Data;

/// <summary>
/// Deletes the specified entity.
/// </summary>
[Cmdlet(VerbsCommon.Remove, "Object"), OutputType(typeof(bool))]
public class RemoveObjectCommand: Cmdlet {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The entity to be deleted.
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
		var entityType = InputObject.GetType();
		var parameterTypes = new[] { typeof(IDbConnection), entityType, typeof(CommandOptions) };
		var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Delete), 1, parameterTypes)!.MakeGenericMethod(entityType);
		var arguments = new object[] { Connection, InputObject, new CommandOptions { Timeout = Timeout, Transaction = Transaction } };
		WriteObject(method.Invoke(null, arguments));
	}
}
