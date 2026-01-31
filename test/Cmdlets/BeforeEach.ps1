using namespace System.Diagnostics.CodeAnalysis

[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
$connection = [System.Data.SQLite.SQLiteConnection]::new("DataSource=:memory:")
$connection.Open()

$command = $connection.CreateCommand()
$command.CommandText = Get-Content "res/Schema.sql" -Raw
$command.ExecuteNonQuery()
$command.Dispose()
