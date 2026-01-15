namespace Belin.Sql.Cmdlets;

using System.Data;

/// <summary>
/// Rolls back the specified database transaction.
/// </summary>
[Cmdlet(VerbsLifecycle.Deny, "Transaction"), OutputType(typeof(void))]
public class DenyTransactionCommand: Cmdlet {

	/// <summary>
	/// The transaction to roll back.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
	public required IDbTransaction InputObject { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => InputObject.Rollback();
}
