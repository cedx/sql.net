namespace Belin.Sql.Cmdlets;

using System.Data;

/// <summary>
/// Commits the specified database transaction.
/// </summary>
[Cmdlet(VerbsLifecycle.Approve, "Transaction"), OutputType(typeof(void))]
public class ApproveTransactionCommand: Cmdlet {

	/// <summary>
	/// The transaction to commit.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
	public required IDbTransaction InputObject { get; set; }

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => InputObject.Commit();
}
