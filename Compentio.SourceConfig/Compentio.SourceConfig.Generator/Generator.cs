using Compentio.SourceConfig.Generators;
using Compentio.SourceConfig.Context;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Compentio.SourceConfig
{
    /// <summary>
    /// Main source generator
    /// </summary>
    [Generator]
    public class Generator : ISourceGenerator
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
            Debug.WriteLine($"'{typeof(Generator).FullName}' initalized.");
        }
    }
}
