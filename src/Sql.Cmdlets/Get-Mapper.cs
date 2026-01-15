namespace Belin.Sql.Cmdlets;

/// <summary>
/// Gets the singleton instance of the data mapper.
/// </summary>
[Cmdlet(VerbsCommon.Get, "Mapper"), OutputType(typeof(Mapper))]
public class GetMapperCommand: Cmdlet {

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => WriteObject(Mapper.Instance);
}
