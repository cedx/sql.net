namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="CommandBuilder"/> class.
/// </summary>
[TestClass]
public sealed class CommandBuilderTests {

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private SQLiteConnection connection = default!;

	/// <summary>
	/// The test data.
	/// </summary>
	private readonly Character record = new() { Id = 1000, FirstName = "Cédric", Gender = CharacterGender.DarkLord };

	[TestInitialize]
	public void TestInitialize() => connection = That.CreateInMemoryDatabase();

	[TestCleanup]
	public void TestCleanup() => connection.Close();

	[TestMethod]
	public void GetDeleteCommand() {
		var command = new CommandBuilder(connection).GetDeleteCommand(record);
		StartsWith(@"DELETE FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = command.Parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}

	[TestMethod]
	public void GetExistsCommand() {
		var command = new CommandBuilder(connection).GetExistsCommand<Character>(record.Id);
		StartsWith("SELECT 1", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = command.Parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}
	
	[TestMethod]
	public void GetFindCommand() {
		var builder = new CommandBuilder(connection);

		var command = builder.GetFindCommand<Character>(record.Id);
		StartsWith(@"SELECT """, command.Text);
		DoesNotContain("*", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameter = command.Parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);

		command = builder.GetFindCommand<Character>(record.Id, ["firstName"]);
		StartsWith(@"SELECT ""firstName""", command.Text);
		DoesNotContain("gender", command.Text);
		DoesNotContain("lastName", command.Text);
	}

	[TestMethod]
	public void GetInsertCommand() {
		var command = new CommandBuilder(connection).GetInsertCommand(record);
		StartsWith(@"INSERT INTO ""main"".""Characters"" (", command.Text);
		Contains("VALUES (", command.Text);

		var parameters = command.Parameters;
		HasCount(3, parameters);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual("", parameters["lastName"].Value);
	}

	[TestMethod]
	public void GetUpdateCommand() {
		var builder = new CommandBuilder(connection);

		var command = builder.GetUpdateCommand(record);
		StartsWith(@"UPDATE ""main"".""Characters""", command.Text);
		Contains(@"SET """, command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		var parameters = command.Parameters;
		HasCount(4, parameters);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual(1000, parameters["ID"].Value);
		AreEqual("", parameters["lastName"].Value);

		HasCount(2, builder.GetUpdateCommand(record, "lastName").Parameters);
	}
}
