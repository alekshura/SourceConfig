# <img src="/Compentio.Assets/Logo.png" align="left" width="50"> SourceConfig


# Introduction
`SourceConfig` is a code generator for objects that buit using `*.json` configuration files: 
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
Lets create you `*json` configuration file (any `json` files are treated as config files), e.g. `apsetting.json` 

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
