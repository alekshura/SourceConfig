using Compentio.SourceConfig.Generator.Generators;
using Compentio.SourceConfig.Generator.Context;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Compentio.SourceConfig.Generator
{
    /// <summary>
    /// Main source generator
    /// </summary>
    [Generator]
    public class ConfigurationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var configMetadata = ConfigurationContext.CreateFromExecutionContext(context);

            foreach (var configFileContext in configMetadata.Context)
            {
                var generator = new CodeGenerator(configFileContext);
                context.AddSource(configFileContext.FileName, generator.GenerateSource());
            }         
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif
            //Debug.WriteLine("Initalize code generator");
        }
    }
}
