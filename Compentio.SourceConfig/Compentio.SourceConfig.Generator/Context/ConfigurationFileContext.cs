using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Compentio.SourceConfig.Generator.Context
{
    /// <summary>
    /// Contains information about one configuration file in te application
    /// </summary>
    interface IConfigurationFileContext
    {
        /// <summary>
        /// Filename of generated POCO file. It is always has <code>*.cs</code> extension.
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// Generated class name
        /// </summary>
        string ClassName { get; }
        /// <summary>
        /// Namespace of generated class. It is concatenation of main application namespaca and directories of configuration files.
        /// </summary>
        string Namespace { get; }
        /// <summary>
        /// Indicated when files should be merged. It is used, when there two or more configuration files defined for different environments:
        /// <code>appsettings.json</code> or <code>appsettings.development.json</code> etc.
        /// </summary>
        /// <param name="filePath">Path to another file that checked for merging</param>
        /// <returns>Flag if two files should be merged</returns>
        bool ShouldBeMerged(string filePath);
        /// <summary>
        /// Deserialized to dictionary content of json file
        /// </summary>
        Dictionary<string, object> FileContent { get; set; }
        /// <summary>
        /// List of top properties in a file
        /// </summary>
        IList<KeyValuePair<string, object>> MainProperties { get; }
        /// <summary>
        /// List of objects (non primitives) from configuration file
        /// </summary>
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
