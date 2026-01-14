namespace Belin.Sql.Cmdlets;

/// <summary>
/// Creates a new data mapper.
/// </summary>
[Cmdlet(VerbsCommon.New, "Mapper"), OutputType(typeof(Mapper))]
public class NewMapperCommand: Cmdlet {

	/// <summary>
	/// Performs execution of this command.
	/// </summary>
	protected override void ProcessRecord() => WriteObject(Mapper.Instance);
}

// TODO rename the cmdlet to `Get-Mapper`
