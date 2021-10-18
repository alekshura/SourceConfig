using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Compentio.SourceConfig.Generator.Context
{
    interface IConfigurationFileContext
    {
        string FileName { get; }
        string ClassName { get; }
        string Namespace { get; }
        bool ShouldBeMerged(string filePath);
        Dictionary<string, object> FileContent { get; set; }
        IList<KeyValuePair<string, object>> MainProperties { get; }
        IList<KeyValuePair<string, object>> ConfigClasses { get; }
    }

    class ConfigurationFileContext : IConfigurationFileContext
    {
        private Dictionary<string, object> _fileContent;
        private readonly string _filePath;
        private readonly string _assemblyName;

        public ConfigurationFileContext(string filePath, string assemblyName, string fileContent)
        {
            _filePath = filePath;
            _fileContent = Deserialize(fileContent);
            _assemblyName = assemblyName;
        }

        public string ClassName => FormatClassName(Path.GetFileNameWithoutExtension(_filePath));
        public string FileName  => $"{ClassName}.cs";
        public IList<KeyValuePair<string, object>> MainProperties => _fileContent
                .Where(dict => !(dict.Value is Dictionary<string, object>))
                .ToList();
        public IList<KeyValuePair<string, object>> ConfigClasses => _fileContent.Except(MainProperties)
                    .ToList();

        public Dictionary<string, object> FileContent 
        { 
            get => _fileContent; 
            set => _fileContent = value; 
        }

        public string Namespace
        {
            get
            {
                var assemlyRootDirectory = _filePath.Split(new string[] { _assemblyName }, StringSplitOptions.RemoveEmptyEntries)[1];
                var namespaceName = string.Empty;
                foreach (var item in assemlyRootDirectory.Split(Path.DirectorySeparatorChar).Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    if (item != Path.GetFileName(_filePath))
                        namespaceName += $".{item}";
                }

                return $"{_assemblyName}{namespaceName}";
            }
        }

        public bool ShouldBeMerged(string filePath)
        {
            var sourceFileName = Path.GetFileNameWithoutExtension(_filePath);
            var targetFileName = Path.GetFileNameWithoutExtension(filePath);
            var sorceOrigin = sourceFileName.Split('.')[0];
            var targetOrigin = targetFileName.Split('.')[0];
            return sorceOrigin.Equals(targetOrigin, StringComparison.InvariantCultureIgnoreCase);
        }

        private string FormatClassName(string className)
        {
            var originName = className.Split('.').Where(item => !string.IsNullOrWhiteSpace(item)).First();

            if (string.IsNullOrWhiteSpace(originName))
                return className.Replace("settings", "Settings");

            return string.Concat(originName[0].ToString().ToUpper(), originName.Substring(1)).Replace("settings", "Settings");
        }

        private Dictionary<string, object> Deserialize(string content)
        {
            var configValues = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content);
            var result = new Dictionary<string, object>();
            foreach (var configValue in configValues)
            {
                if (configValue.Value.ValueKind is JsonValueKind.Object)
                {
                    result.Add(configValue.Key, Deserialize(configValue.Value.ToString()));
                }
                else
                {
                    result.Add(configValue.Key, configValue.Value);
                }
            }

            return result;
        }
    }
}
