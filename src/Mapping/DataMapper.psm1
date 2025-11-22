using namespace System.Collections
using namespace System.ComponentModel.DataAnnotations.Schema
using namespace System.Diagnostics.CodeAnalysis
using namespace System.Reflection

<#
.SYNOPSIS
	Maps data records to entity objects.
#>
class DataMapper {

	<#
	.SYNOPSIS
		The property maps, keyed by type.
	#>
	hidden static [hashtable] $PropertyMaps = @{}

	<#
	.SYNOPSIS
		Creates a new object of a given type from the specified data record.
	.PARAMETER Type
		The object type.
	.PARAMETER Record
		A data record providing the properties to be set on the created object.
	.OUTPUTS
		The newly created object.
	#>
	[SuppressMessage("PSUseDeclaredVarsMoreThanAssignments", "")]
	[object] CreateInstance([type] $Type, [System.Data.IDataRecord] $Record) {
		$properties = @{}
		for ($index = 0; $index -lt $Record.FieldCount; $index++) {
			$key = $Record.GetName($index)
			$properties.$key = $Record.IsDBNull($index) ? $null : $Record.GetValue($index)
		}

		return $discard = switch ($Type) {
			([hashtable]) { $properties; break }
			([psobject]) { [pscustomobject] $properties; break }
			default { $this.CreateInstance($Type, $properties) }
		}
	}

	<#
	.SYNOPSIS
		Creates a new object of a given type from the specified hash table.
	.PARAMETER Type
		The object type.
	.PARAMETER Properties
		A hash table providing the properties to be set on the created object.
	.OUTPUTS
		The newly created object.
	#>
	[object] CreateInstance([type] $Type, [hashtable] $Properties) {
		$culture = [cultureinfo]::InvariantCulture
		$object = $Type::new()
		$propertyMap = $this.GetPropertyMap($Type)

		foreach ($key in $Properties.Keys.Where{ $_ -in $propertyMap.Keys }) {
			$propertyInfo = $propertyMap.$key
			$propertyType = [Nullable]::GetUnderlyingType($propertyInfo.PropertyType) ?? $propertyInfo.PropertyType
			$value = $Properties.$key

			$object.$($propertyInfo.Name) = switch ($true) {
				($null -eq $value) { $null; break }
				($propertyType.IsEnum) { [Enum]::ToObject($propertyType, $value); break }
				default { [Convert]::ChangeType($value, $propertyType, $culture) }
			}
		}

		return $object
	}

	<#
	.SYNOPSIS
		Creates new objects of a given type from the specified data reader.
	.PARAMETER Type
		The object type.
	.PARAMETER Reader
		A data reader providing the properties to be set on the created objects.
	.OUTPUTS
		An array of newly created objects.
	#>
	[object[]] CreateInstances([type] $Type, [System.Data.IDataReader] $Reader) {
		$list = [ArrayList]::new()
		while ($Reader.Read()) { $list.Add($this.CreateInstance($Type, $Reader)) }
		$Reader.Close()
		return $list.ToArray()
	}

	<#
	.SYNOPSIS
		Retrives a hash table of mapped properties of the specified type.
	.PARAMETER Type
		The type to inspect.
	.OUTPUTS
		The hash table of mapped properties of the specified type.
	#>
	[hashtable] GetPropertyMap([type] $Type) {
		if ($Type -in [DataMapper]::PropertyMaps.Keys) { return [DataMapper]::PropertyMaps.$Type }

		$propertyMap = @{}
		$propertyInfos = $Type.GetProperties([BindingFlags]::Instance -bor [BindingFlags]::Public)
		foreach ($propertyInfo in $propertyInfos.Where{ $_.CanWrite -and (-not [Attribute]::IsDefined($_, ([NotMappedAttribute])))}) {
			$column = [Attribute]::GetCustomAttribute($propertyInfo, ([ColumnAttribute]))
			$propertyMap.$($column ? $column.Name : $propertyInfo.Name) = $propertyInfo
		}

		return [DataMapper]::PropertyMaps.$Type = $propertyMap
	}
}
