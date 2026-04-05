using namespace System.Data.SQLite
using namespace System.Diagnostics.CodeAnalysis

[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
$connection = [SQLiteConnection]::new("DataSource=:memory:")
$connection.Open()

$command = $connection.CreateCommand()
$command.CommandText = Get-Content "res/Schema.sql" -Raw
$command.ExecuteNonQuery()
$command.Dispose()
