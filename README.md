# <img src="/Compentio.Assets/Logo.png" align="left" width="50"> SourceConfig


[![NuGet](http://img.shields.io/nuget/v/Compentio.SourceConfig.svg)](https://www.nuget.org/packages/Compentio.SourceConfig)
![Nuget](https://img.shields.io/nuget/dt/Compentio.SourceConfig)
![GitHub](https://img.shields.io/github/license/alekshura/SourceConfig)
![GitHub top language](https://img.shields.io/github/languages/top/alekshura/SourceConfig)

# Introduction
`SourceConfig` is a code generator for objects that are built on `*.json` configuration files: 
when developer adds some file or new properties to existng json configuration file the POCO objects for this configuration generated. 

It is based on [Source Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md) feature
that has been intoduced with `C# 9.0` and brings a possibility to  generate code during build time.


# Installation
Install using nuget package manager:

```console
Install-Package Compentio.SourceConfig
```

or `.NET CLI`:

```console
dotnet add package Compentio.SourceConfig
```

# How to use
During creation of any `*json` file (any `json` files are treated as configuration files), e.g. `apsetting.json` 
the POCO representation of this json is generated: 

```json
{
  "NoteEmailAddresses": [
    "admin@test.com",
    "technical.admin@test.com",
    "business.admin@test.com"
  ],
  "ConnectionTimeout": "30",
  "ConnectionHost": "https://test.com",
  "DefaultNote": {
    "Title": "DefaultTitle",
    "Description":  "DefaultDescription"
  }
}
```
in that case `SourceConfig` generates

```cs
// <mapper-source-generated />
// <generated-at '18.10.2021 14:49:51' />
using System;
using System.Collections.Generic;

namespace Compentio.SourceConfig.App
{
    public class AppSettings
    {
        public IEnumerable<string> NoteEmailAddresses { get; set; }

        public string ConnectionTimeout { get; set; }

        public string ConnectionHost { get; set; }

        public string DatabaseSize { get; set; }

        public DefaultNote DefaultNote { get; set; }
    }

    public class DefaultNote
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
```
`AppSettings` is taken from the filename, `Compentio.SourceConfig.App` namespace is inherited configuration file directory (here, `appsettings.json` is in app root directory,
thus main app namespace is used).

>In `*.cproj` project file the configuration files should be marked as `AdditionalFiles`:
>```xml
><ItemGroup>
>    <AdditionalFiles Include="Appsettings.Development.json">
>      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
>    </AdditionalFiles>
>    <AdditionalFiles Include="Appsettings.json">
>      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
>    </AdditionalFiles>
>  </ItemGroup>

If there are few `appsettings` files used for different environments, e.g. `appsettings.development.json` or `appsettings.production.json`
they are merged into one generated class. Merge is based on first prefix in filename - here `appsettings`.

Now generated class can be used to retreive the configuration:

```cs
var appSettings = _configuration.Get<AppSettings>();
```
and should be earlier added to container:

```cs
static IHostBuilder CreateHostBuilder(string[] args)
{
    var configuration = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables()
       .Build();

    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
            services
            .Configure<AppSettings>(configuration)
            .AddTransient<INotesService, NotesService>());
}
```

