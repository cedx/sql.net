namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Reflection;

/// <summary>
/// Executes a parameterized SQL query that selects a single value.
/// </summary>
[Cmdlet(VerbsCommon.Get, "Scalar"), OutputType(typeof(object))]
public class GetScalarCommand: Cmdlet {

	/// <summary>
	/// The SQL query to be executed.
	/// </summary>
	[Parameter(Mandatory = true, Position = 1)]
	public required string Command { get; set; }

	/// <summary>
	/// Value indicating how the command is interpreted.
	/// </summary>
	[Parameter]
	public CommandType CommandType { get; set; } = CommandType.Text;

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	[Parameter(Mandatory = true, Position = 0)]
	public required IDbConnection Connection { get; set; }

	/// <summary>
	/// The parameters of the SQL query.
	/// </summary>
	[Parameter(Position = 2)]
	public ParameterCollection Parameters { get; set; } = [];

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
		Type[] types = [typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(CommandOptions)];
		object?[] arguments = [Connection, Command, Parameters, new CommandOptions { Timeout = Timeout, Transaction = Transaction, Type = CommandType }];

		try {
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.ExecuteScalar), 1, types)!.MakeGenericMethod(typeof(object));
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Get-Scalar:TargetInvocationException", ErrorCategory.OperationStopped, null));
		}
	}
}
