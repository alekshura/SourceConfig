using Compentio.SourceConfig.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Compentio.SourceConfig.Generators
{
    /// <summary>
    /// One class code generator
    /// </summary>
    interface ICodeGenerator
    {
        /// <summary>
        /// Generates configuration POCO object for one file
        /// </summary>
        /// <returns>Generated and formatted code</returns>
        SourceText GenerateSource();
    }

    class CodeGenerator : ICodeGenerator
    {
        private readonly IConfigurationFileContext _configurationFileContext;
        
        public CodeGenerator(IConfigurationFileContext configurationFileContext)
        {
            _configurationFileContext = configurationFileContext;
        }

        public SourceText GenerateSource()
        {
            Debug.WriteLine($"Start generating sources for '{_configurationFileContext.ClassName}' class.");

            var configSectionClasses = new StringBuilder();
            
            foreach (var configClass in _configurationFileContext.ConfigClasses)
            {
                BuildConfigClass(configClass, configSectionClasses);
            }

            var sourceBuilder = new StringBuilder($@"
                // <mapper-source-generated />
                // <generated-at '{DateTime.UtcNow}' />

                using System;
                using System.Collections.Generic;
                using System.Diagnostics.CodeAnalysis;

                namespace {_configurationFileContext.Namespace}
                {{
                    [ExcludeFromCodeCoverage]
                    public class {_configurationFileContext.ClassName}
                    {{
                ");

            foreach (var item in _configurationFileContext.MainProperties)
            {
                var value = item.Value;
                var key = item.Key;

                if (value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    var propertyType = GetPropertyTypeName(element.EnumerateArray().FirstOrDefault());
                    sourceBuilder.Append($"public IEnumerable<{propertyType}> {FormatPropertyName(key)} {{ get; set; }}");
                }
                else
                {
                    sourceBuilder.Append($"public string {FormatPropertyName(key)} {{ get; set; }}");
                }
            }

            foreach (var item in _configurationFileContext.ConfigClasses)
            {
                var key = item.Key;
                sourceBuilder.Append($"public {FormatPropertyName(key)} {FormatPropertyName(key)}{{ get; set; }}");
            }

            sourceBuilder.Append("}");
            sourceBuilder.Append(configSectionClasses.ToString());
            sourceBuilder.Append("}");

            var tree = CSharpSyntaxTree.ParseText(sourceBuilder.ToString());

            Debug.WriteLine($"End generating sources for '{_configurationFileContext.ClassName}' class. Success!");
            return SourceText.From(tree.GetRoot().NormalizeWhitespace().ToFullString(), Encoding.UTF8);
        }

        private void BuildConfigClass(KeyValuePair<string, object> classInfo, StringBuilder stringBuilder)
        {
            var nestedClasses = new StringBuilder();

            stringBuilder.Append($"public class {classInfo.Key}");
            stringBuilder.Append("{");

            foreach (var item in (Dictionary<string, object>)classInfo.Value)
            {
                if (item.Value is Dictionary<string, object>)
                {
                    stringBuilder.Append($"public {item.Key} {FormatPropertyName(item.Key)} {{ get; set; }}");
                    BuildConfigClass(item, nestedClasses);
                }
                else
                {
                    var prop = (JsonElement)item.Value;
                    var propertyType = GetPropertyTypeName(prop);
                    if (prop.ValueKind == JsonValueKind.Array)
                    {
                        stringBuilder.Append($"public IEnumerable<{propertyType}> {FormatPropertyName(item.Key)} {{ get; set; }}");
                    }
                    else
                    {
                        stringBuilder.Append($"public {propertyType} {FormatPropertyName(item.Key)} {{ get; set; }}");
                    }
                }
            }

            stringBuilder.Append("}");
            stringBuilder.AppendLine(nestedClasses.ToString());
        }

        private string GetPropertyTypeName(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.Number => "int",
                JsonValueKind.True or JsonValueKind.False => "bool",
                _ => "string",
            };
        }

        private string FormatPropertyName(string origin)
        {
            var underscore = "_";
            return origin
                .Replace(".", underscore)
                .Replace("$", underscore);
        }
    }
}
