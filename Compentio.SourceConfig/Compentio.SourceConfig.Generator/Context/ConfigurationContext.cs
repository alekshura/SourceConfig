using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Compentio.SourceConfig.Generator.Context
{
     /// <summary>
     /// Contains for overall configurations that defined in json files in application
     /// </summary>
    interface IConfigurationContext
    {
        /// <summary>
        /// Collection of configuration files information
        /// </summary>
        IEnumerable<IConfigurationFileContext> Context { get; }
    }

    class ConfigurationContext : IConfigurationContext
    {
        private readonly GeneratorExecutionContext _generatorExecutionContext;
        private readonly IList<IConfigurationFileContext> _configFilesContext;       

        public static IConfigurationContext CreateFromExecutionContext(GeneratorExecutionContext generatorExecutionContext) => new ConfigurationContext(generatorExecutionContext);

        private ConfigurationContext(GeneratorExecutionContext generatorExecutionContext)
        {
            _generatorExecutionContext = generatorExecutionContext;
            _configFilesContext = new List<IConfigurationFileContext>();
            LoadConfigFiles(_configFilesContext);
        }

        public IEnumerable<IConfigurationFileContext> Context => _configFilesContext;

        private void LoadConfigFiles(IList<IConfigurationFileContext> configFilesContext)
        {
            foreach (var configFile in _generatorExecutionContext.AdditionalFiles.Where(file => file.Path.EndsWith(".json")))
            {
                var content = configFile.GetText()?.ToString();
                if (!string.IsNullOrEmpty(content))
                {
                    var fileContext = new ConfigurationFileContext(configFile.Path, _generatorExecutionContext.Compilation.AssemblyName, content);
                    var fileToMerge = configFilesContext.FirstOrDefault(file => file.ShouldBeMerged(configFile.Path));
                    if (fileToMerge is not null)
                    {
                        Merge(fileToMerge.FileContent, fileContext.FileContent);
                    }
                    else
                    {
                        configFilesContext.Add(fileContext);
                    }                    
                }
            }
        }

        private void Merge(Dictionary<string, object> result, Dictionary<string, object> source)
        {
            foreach (var entry in source)
            {
                if (!result.ContainsKey(entry.Key))
                {
                    result.Add(entry.Key, entry.Value);
                }
                else
                {
                    if (entry.Value is Dictionary<string, object> existing)
                    {
                        int numberOfValuesInExistingObject = existing.Count;
                        var numberOfValuesInNewObject = ((Dictionary<string, object>)result[entry.Key]).Count;

                        if (numberOfValuesInExistingObject < numberOfValuesInNewObject)
                        {
                            result[entry.Key] = entry.Value;
                            Merge((Dictionary<string, object>)result[entry.Key], (Dictionary<string, object>)entry.Value);
                        }
                    }
                }
            }
        }
    }
}
