namespace Belin.Sql;

using Belin.Sql.Fixtures;
using System.Data.SQLite;

/// <summary>
/// Tests the features of the <see cref="SqlCommandBuilder"/> class.
/// </summary>
[TestClass]
public sealed class SqlCommandBuilderTests {

	/// <summary>
	/// The test data.
	/// </summary>
	private readonly Character character = new() { Id = 1000, FirstName = "Cédric", Gender = CharacterGender.DarkLord };

	/// <summary>
	/// The connection to the data source.
	/// </summary>
	private readonly SQLiteConnection connection = new("DataSource=:memory:");

	[TestMethod]
	public void GetDeleteCommand() {
		// It should return the SQL command to delete an entity.
		var (command, parameters) = new SqlCommandBuilder(connection).GetDeleteCommand(character);
		StartsWith(@"DELETE FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}

	[TestMethod]
	public void GetExistsCommand() {
		// It should return the SQL command to check the existence of an entity.
		var (command, parameters) = new SqlCommandBuilder(connection).GetExistsCommand<Character>(character.Id);
		StartsWith("SELECT 1", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);
	}

	[TestMethod]
	public void GetFindCommand() {
		var builder = new SqlCommandBuilder(connection);

		// It should return the SQL command to find an entity.
		var (command, parameters) = builder.GetFindCommand<Character>(character.Id);
		StartsWith(@"SELECT """, command.Text);
		DoesNotContain("*", command.Text);
		Contains(@"FROM ""main"".""Characters""", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		var parameter = parameters.Single();
		AreEqual("@ID", parameter.Name);
		AreEqual(1000, parameter.Value);

		// It should allow selecting a specific set of columns.
		(command, _) = builder.GetFindCommand<Character>(character.Id, ["firstName"]);
		StartsWith(@"SELECT ""firstName""", command.Text);
		DoesNotContain("gender", command.Text);
		DoesNotContain("lastName", command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);
	}

	[TestMethod]
	public void GetInsertCommand() {
		// It should return the SQL command to insert an entity.
		var (command, parameters) = new SqlCommandBuilder(connection).GetInsertCommand(character);
		StartsWith(@"INSERT INTO ""main"".""Characters"" (", command.Text);
		Contains("VALUES (", command.Text);

		// It should also return the parameters used by the SQL command.
		HasCount(3, parameters);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual("", parameters["lastName"].Value);
	}

	[TestMethod]
	public void GetUpdateCommand() {
		var builder = new SqlCommandBuilder(connection);

		// It should return the SQL command to update an entity.
		var (command, parameters) = builder.GetUpdateCommand(character);
		StartsWith(@"UPDATE ""main"".""Characters""", command.Text);
		Contains(@"SET """, command.Text);
		EndsWith(@"WHERE ""ID"" = @ID", command.Text);

		// It should also return the parameters used by the SQL command.
		HasCount(4, parameters);
		AreEqual(1000, parameters["ID"].Value);
		AreEqual("Cédric", parameters["firstName"].Value);
		AreEqual(CharacterGender.DarkLord, parameters["gender"].Value);
		AreEqual("", parameters["lastName"].Value);

		// It should allow updating a specific set of columns.
		(_, parameters) = builder.GetUpdateCommand(character, "firstName");
		HasCount(2, parameters);
		AreEqual(1000, parameters["ID"].Value);
		AreEqual("Cédric", parameters["firstName"].Value);
	}
}
