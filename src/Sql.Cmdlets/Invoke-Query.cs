namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Dynamic;
using System.Reflection;

/// <summary>
/// Executes a parameterized SQL query and returns a sequence of objects whose properties correspond to the columns.
/// </summary>
[Cmdlet(VerbsLifecycle.Invoke, "Query"), OutputType(typeof(object), typeof((object?, object?)), typeof(IEnumerable<object>), typeof(IEnumerable<(object?, object?)>))]
public class InvokeQueryCommand: Cmdlet {

	/// <summary>
	/// An array of types representing the number, order, and type of the parameters of the underlying method to invoke.
	/// </summary>
	private static readonly Type[][] parameterTypes = [
		[typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(QueryOptions)],
		[typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(string), typeof(QueryOptions)],
		[typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof((string, string)?), typeof(QueryOptions)],
		[typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof((string, string, string)?), typeof(QueryOptions)]
	];

	/// <summary>
	/// The type of objects to return.
	/// </summary>
	[Parameter, ValidateCount(1, 4)]
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
	/// Value indicating whether to prevent from enumerating the rows.
	/// </summary>
	[Parameter]
	public SwitchParameter NoEnumerate { get; set; }

	/// <summary>
	/// The parameters of the SQL query.
	/// </summary>
	[Parameter(Position = 2)]
	public ParameterCollection Parameters { get; set; } = [];

	/// <summary>
	/// The field from which to split and read the next object.
	/// </summary>
	[Parameter, ValidateCount(1, 3), ValidateNotNullOrWhiteSpace]
	public string[] SplitOn { get; set; } = ["Id"];

	/// <summary>
	/// Value indicating whether to prevent from buffering the rows in memory.
	/// </summary>
	[Parameter]
	public SwitchParameter Stream { get; set; }

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
		var queryOptions = new QueryOptions { Buffered = !Stream, Timeout = Timeout, Transaction = Transaction, Type = CommandType };
		object[] arguments = As.Length switch {
			1 => [Connection, Command, Parameters, queryOptions],
			2 => [Connection, Command, Parameters, SplitOn[0], queryOptions],
			3 => [Connection, Command, Parameters, ValueTuple.Create(SplitOn[0], SplitOn.Length <= 1 ? "Id" : SplitOn[1]), queryOptions],
			_ => [Connection, Command, Parameters, ValueTuple.Create(SplitOn[0], SplitOn.Length <= 1 ? "Id" : SplitOn[1], SplitOn.Length <= 2 ? "Id" : SplitOn[2]), queryOptions]
		};

		try {
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.Query), As.Length, parameterTypes[As.Length - 1])!.MakeGenericMethod(As);
			WriteObject(method.Invoke(null, arguments), enumerateCollection: !NoEnumerate);
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Invoke-Query:TargetInvocationException", ErrorCategory.OperationStopped, null));
		}
	}
}
