namespace Belin.Sql.Cmdlets;

using System.Data;
using System.Dynamic;
using System.Reflection;

/// <summary>
/// Executes a parameterized SQL query and returns the first row.
/// </summary>
[Cmdlet(VerbsCommon.Get, "First"), OutputType(typeof(object))]
public class GetFirstCommand: Cmdlet {

	/// <summary>
	/// An array of types representing the number, order, and type of the parameters of the underlying method to invoke.
	/// </summary>
	private static readonly Type[] parameterTypes = [typeof(IDbConnection), typeof(string), typeof(ParameterCollection), typeof(CommandOptions)];

	/// <summary>
	/// The type of objects to return.
	/// </summary>
	[Parameter]
	public Type As { get; set; } = typeof(ExpandoObject);

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
		try {
			var method = typeof(ConnectionExtensions).GetMethod(nameof(ConnectionExtensions.QueryFirst), 1, parameterTypes)!.MakeGenericMethod(As);
			var arguments = new object[] { Connection, Command, Parameters, new CommandOptions { Timeout = Timeout, Transaction = Transaction, Type = CommandType } };
			WriteObject(method.Invoke(null, arguments));
		}
		catch (TargetInvocationException e) {
			WriteError(new ErrorRecord(e.InnerException, "Get-First:TargetInvocationException", ErrorCategory.InvalidOperation, null));
		}
	}
}
