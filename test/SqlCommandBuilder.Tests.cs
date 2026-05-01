namespace Belin.Sql;

using Belin.Sql.Fixtures;

/// <summary>
/// Tests the features of the <see cref="SqlCommandBuilder"/> class.
/// </summary>
[TestClass]
public sealed class SqlCommandBuilderTests: DataSourceTests {

	/// <summary>
	/// The test data.
	/// </summary>
	private readonly Character record = new() { Id = 1000, FirstName = "Cédric", Gender = CharacterGender.DarkLord };

	[TestMethod]
	public void GetDeleteCommand() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetDeleteCommand(record);
		StartsWith(@"DELETE FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}

	[TestMethod]
	public void GetExistsCommand() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetExistsCommand<Character>(record.Id);
		StartsWith("SELECT 1", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}

	[TestMethod]
	public void GetFindCommand() {
		var builder = new SqlCommandBuilder(connection);

		var (command, parameters) = builder.GetFindCommand<Character>(record.Id);
		StartsWith(@"SELECT """, command.Text);
		DoesNotContain("*", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);

		(command, _) = builder.GetFindCommand<Character>(record.Id, ["firstName"]);
		StartsWith(@"SELECT ""firstName""", command.Text);
		DoesNotContain("gender", command.Text);
		DoesNotContain("lastName", command.Text);
	}

	[TestMethod]
	public void GetInsertCommand() {
		var (command, parameters) = new SqlCommandBuilder(connection).GetInsertCommand(record);
		StartsWith(@"INSERT INTO ""main"".""Characters"" (", command.Text);
		Contains("VALUES (", command.Text);

		HasCount(3, parameters);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual("", parameters["lastName"].Value);
	}

	[TestMethod]
	public void GetUpdateCommand() {
		var builder = new SqlCommandBuilder(connection);

		var (command, parameters) = builder.GetUpdateCommand(record);
		StartsWith(@"UPDATE ""main"".""Characters""", command.Text);
		Contains(@"SET """, command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		HasCount(4, parameters);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual(1000, parameters["ID"].Value);
		AreEqual("", parameters["lastName"].Value);

		HasCount(2, builder.GetUpdateCommand(record, "lastName").Parameters);
	}
}
