using namespace Belin.Sql
using namespace System.Data

<#
.SYNOPSIS
	Executes a parameterized SQL query and returns a data reader.
.OUTPUTS
	The data reader that can be used to access the results.
#>
function Invoke-Reader {
	[CmdletBinding()]
	[OutputType([System.Data.IDataReader])]
	param (
		# The connection to the data source.
		[Parameter(Mandatory, Position = 0)]
		[IDbConnection] $Connection,

		# The SQL query to be executed.
		[Parameter(Mandatory, Position = 1)]
		[string] $Command,

		# The parameters of the SQL query.
		[Parameter(Position = 2)]
		[ParameterCollection] $Parameters = @(),

		# Value indicating how the command is interpreted.
		[CommandType] $CommandType = [CommandType]::Text,

		# The wait time, in seconds, before terminating the attempt to execute the command and generating an error.
		[ValidateRange("Positive")]
		[int] $Timeout = 30,

		# The transaction to use, if any.
		[IDbTransaction] $Transaction
	)

	$commandOptions = [CommandOptions]@{ Timeout = $Timeout; Transaction = $Transaction; Type = $CommandType }
	$reader = [ConnectionExtensions]::ExecuteReader($Connection, $Command, $Parameters, $commandOptions)
	Write-Output $reader -NoEnumerate
}
