namespace Belin.Sql.Cmdlets;

using Belin.Sql.Mapping;
using System.Data;

/// <summary>
/// Creates a new data mapper.
/// </summary>
[Cmdlet(VerbsCommon.New, "DataMapper")]
[OutputType(typeof(DataMapper))]
public class NewDataMapper: Cmdlet {

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => WriteObject(new DataMapper());
}
