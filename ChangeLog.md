# Changelog

## Version [3.0.0](https://github.com/cedx/sql.net/compare/v2.0.0...v3.0.0)
- Breaking change: removed some parameters from the `Parameter` primary constructor.
- Breaking change: renamed the `New-Mapper` cmdlet to `Get-Mapper`.
- Breaking change: transformed the `Mapper` class into a singleton instance.
- Added new implicit conversions to the `Parameter` class.
- Added a new overload to the `ParameterCollection` constructor.
- Made the `Mapper.GetTable()` method public.

## Version [2.0.0](https://github.com/cedx/sql.net/compare/v1.2.0...v2.0.0)
- Breaking change: the `Query` and `QueryAsync` methods of the `ConnectionExtensions` class now return a `List<T>` by default.
- Breaking change: transformed the `CommandOptions` constructor into a parameterless constructor.
- Added the `QueryOptions` record.
- Added the `IEnumerable<T>.AsList()` extension method.
- Added the `-NoEnumerate` and `-Stream` parameters to the `Invoke-Query` cmdlet.

## Version [1.2.0](https://github.com/cedx/sql.net/compare/v1.1.0...v1.2.0)
- Simplified the constraint on generic type parameters.

## Version [1.1.0](https://github.com/cedx/sql.net/compare/v1.0.0...v1.1.0)
- Added support for mapping a single record to multiple objects.
- Added the `-SplitOn` parameter to the `Invoke-Query` cmdlet.

## Version [1.0.0](https://github.com/cedx/sql.net/compare/v0.17.0...v1.0.0)
- First stable release.
- Removed the `Belin.Sql.Dapper` assembly.

## Version [0.17.0](https://github.com/cedx/sql.net/compare/v0.16.0...v0.17.0)
- Replaced all `dynamic` references by the `System.Dynamic.ExpandoObject` type.

## Version [0.16.0](https://github.com/cedx/sql.net/compare/v0.15.1...v0.16.0)
- Fixed the handling of parameter names and values.
- Fixed the `New-Parameter` cmdlet.
- The `-Value` parameter of the `New-Parameter` cmdlet is now mandatory.

## Version [0.15.1](https://github.com/cedx/sql.net/compare/v0.15.0...v0.15.1)
- Fixed the handling of parameter names and values.

## Version [0.15.0](https://github.com/cedx/sql.net/compare/v0.14.0...v0.15.0)
- Added an indexer to the `ParameterCollection` class.
- Added the `Contains()`, `IndexOf()` and `RemoveAt()` methods to the `ParameterCollection` class.

## Version [0.14.0](https://github.com/cedx/sql.net/compare/v0.13.1...v0.14.0)
- Added overloads to the constructor of `ParameterCollection` class.
- Renamed the `Parameter.ParameterName` property to `Name`.
- The `Parameter` constructor now allows values to be passed to the `DbType` and `Size` properties.

## Version [0.13.1](https://github.com/cedx/sql.net/compare/v0.13.0...v0.13.1)
- The asynchronous methods of the `ConnectionExtensions` class can now be used with instances of the `IDbConnection` interface.

## Version [0.13.0](https://github.com/cedx/sql.net/compare/v0.12.0...v0.13.0)
- Added non-generic overloads to the methods of `ConnectionExtensions` class.
- Fixed the `Invoke-Query` and `Invoke-Reader` cmdlets.
- Removed the `Adapter` class.
- Removed the `Get-Version` cmdlet.

## Version [0.12.0](https://github.com/cedx/sql.net/compare/v0.11.0...v0.12.0)
- Added implicit conversions to the `Parameter` class.

## Version [0.11.0](https://github.com/cedx/sql.net/compare/v0.10.0...v0.11.0)
- Removed the `ConnectionExtensions.ExecuteScalar()` non-generic method.
- The `Adapter` class now implements the `IDisposable` interface.
- The `Get-Scalar` and `Invoke-Query` cmdlets now use a non-terminating error.

## Version [0.10.0](https://github.com/cedx/sql.net/compare/v0.9.0...v0.10.0)
- Added the `TableInfo` and `ColumnInfo` classes.
- The `Get-First` and `Get-Single` cmdlets now use a non-terminating error.

## Version [0.9.0](https://github.com/cedx/sql.net/compare/v0.8.0...v0.9.0)
- Removed the `CommandExtensions` and `ListExtensions` classes.
- Renamed the `DataMapper` class to `Mapper`.
- Renamed the `QueryOptions` record to `CommandOptions`.
- Renamed the `New-DataMapper` cmdlet to `New-Mapper`.
- Merged the `-Parameters` and `-PositionalParameters` cmdlet parameters.
- Added the `New-Transaction`, `Approve-Transaction` and `Deny-Transaction` cmdlets.
- Added the `-Precision`, `-Scale` and `-Size` parameters to the `New-Parameter` cmdlet.
- Added the `-Transaction` parameter to most cmdlets.

## Version [0.8.0](https://github.com/cedx/sql.net/compare/v0.7.0...v0.8.0)
- Moved the data querying and mapping to the `Belin.Sql` assembly.
- Moved the cmdlets to the `Belin.Sql.Cmdlets` assembly.
- Added the `Belin.Sql.Dapper` assembly providing type maps and type handlers for [Dapper](https://www.learndapper.com).
- Added the `-CommandType` parameter to most cmdlets.
- Added the `-DbType` and `-Direction` parameters to the `New-Parameter` cmdlet.

## Version [0.7.0](https://github.com/cedx/sql.net/compare/v0.6.0...v0.7.0)
- Ported the cmdlets to [C#](https://learn.microsoft.com/en-us/dotnet/csharp).

## Version [0.6.0](https://github.com/cedx/sql.net/compare/v0.5.1...v0.6.0)
- Added the `New-DataMapper` cmdlet.
- Fixed the deserialization of `Enum` and `Nullable` types.

## Version [0.5.1](https://github.com/cedx/sql.net/compare/v0.5.0...v0.5.1)
- Fixed the `New-Command` cmdlet.

## Version [0.5.0](https://github.com/cedx/sql.net/compare/v0.4.0...v0.5.0)
- Added the `-PositionalParameters` argument to most cmdlets.
- Restored the type of the `-Parameters` argument to `[hashtable]`.

## Version [0.4.0](https://github.com/cedx/sql.net/compare/v0.3.0...v0.4.0)
- Changed the type of the `-Parameters` argument to `[System.Collections.Specialized.OrderedDictionary]`.

## Version [0.3.0](https://github.com/cedx/sql.net/compare/v0.2.0...v0.3.0)
- Added the `-Timeout` parameter to most cmdlets.
- Renamed the `Get-ServerVersion` cmdlet to `Get-Version`.

## Version [0.2.0](https://github.com/cedx/sql.net/compare/v0.1.0...v0.2.0)
- Added a simple object mapping feature.
- Added new cmdlets: `Get-ServerVersion`, `New-Command` and `New-Parameter`.
- Replaced the `-AsHastable` parameter of `Get-First`, `Get-Single` and `Invoke-Query` cmdlets by the `-As` parameter.

## Version 0.1.0
- Initial release.
