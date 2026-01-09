namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Dynamic;
using System.Reflection;

/// <summary>
/// Executes a parameterized SQL query and returns an array of objects whose properties correspond to the columns.
/// </summary>
[Cmdlet(VerbsLifecycle.Invoke, "Query"), OutputType(typeof(object), typeof(Tuple<object?, object?>))]
public class InvokeQueryCommand: Cmdlet {

	/// <summary>
	/// The type of objects to return.
	/// </summary>
	[Parameter, ValidateNotNullOrEmpty]
	public Type[] As { get; set; } = [typeof(ExpandoObject)];

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
	/// The field from which to split and read a second object.
	/// </summary>
	[Parameter, ValidateNotNullOrWhiteSpace]
	public string SplitOn { get; set; } = "Id";

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
		Type[] types = As.Length <= 1
			? [typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(CommandOptions)]
			: [typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(string), typeof(CommandOptions)];

		object?[] arguments = As.Length <= 1
			? [Connection, Command, Parameters, new CommandOptions(Timeout, Transaction, CommandType)]
			: [Connection, Command, Parameters, SplitOn, new CommandOptions(Timeout, Transaction, CommandType)];

		try {
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Query), As.Length, types)!.MakeGenericMethod(As);
			WriteObject(method.Invoke(null, arguments), enumerateCollection: true);
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Invoke-Query:TargetInvocationException", ErrorCategory.OperationStopped, null));
		}
	}
}
