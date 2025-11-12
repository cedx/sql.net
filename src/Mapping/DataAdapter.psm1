using module ./DataMapper.psm1

<#
.SYNOPSIS
	Encapsulates a data reader and a data mapper into a single type.
#>
class DataAdapter {

	<#
	.SYNOPSIS
		The data mapper that can be used to convert the records returned by the reader.
	#>
	[ValidateNotNull()]
	[DataMapper] $Mapper

	<#
	.SYNOPSIS
		The data reader that can be used to iterate over the results of the SQL query.
	#>
	[ValidateNotNull()]
	[System.Data.IDataReader] $Reader
}
