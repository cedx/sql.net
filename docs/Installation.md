# Installation

## Requirements
Before installing **SQL Mapper for .NET**, you need to make sure you have the [.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/sdk)
and/or [PowerShell](https://learn.microsoft.com/en-us/powershell) up and running.
		
You can verify if you're already good to go with the following command:

```shell
dotnet --version
# 10.0.201

pwsh --version
# PowerShell 7.6.0
```

## Installing the .NET library with NuGet package manager

### 1. Install it
From a command prompt, run:

```shell
dotnet package add Belin.Sql
```

### 2. Import it
Now in your [C#](https://learn.microsoft.com/en-us/dotnet/csharp) code, you can use:

```cs
using Belin.Sql;
```

## Installing the PowerShell module with PSResourceGet package manager

### 1. Install it
From a command prompt, run:

```powershell
Install-PSResource Belin.Sql -Repository PSGallery
```

### 2. Import it
Now in your [PowerShell](https://learn.microsoft.com/en-us/powershell) code, you can use:

```powershell
Import-Module Belin.Sql
```
