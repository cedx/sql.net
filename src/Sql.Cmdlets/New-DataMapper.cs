namespace Belin.Sql.Cmdlets;

/// <summary>
/// Creates a new data mapper.
/// </summary>
[Cmdlet(VerbsCommon.New, "DataMapper"), OutputType(typeof(Mapper))]
public class NewDataMapperCommand: Cmdlet {

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => WriteObject(new Mapper());
}
