using Microsoft.CodeAnalysis;

namespace Damdor.Statio.CodeGenerator
{
    /// <summary>
    /// Generates Statio parameter registration on editor load and after runtime assemblies load.
    /// </summary>
    [Generator]
    public class StatioSettingsCodeGenerator : ClassRegisterViaAttributeCodeGenerator
    {
        /// <summary>
        /// The attribute name used to discover Statio parameter classes.
        /// </summary>
        public const string StatioParameterAttribute = "StatioParameter";

        /// <inheritdoc/>
        public override string[] Attributes { get; } = { StatioParameterAttribute };

        /// <inheritdoc/>
        public override void Execute(GeneratorExecutionContext context)
        {
            GenerateForAttribute(context, StatioParameterAttribute, (sb, type) =>
            {
                sb.AppendLine($"        global::Damdor.Statio.StatioSettings.RegisterParameterType(typeof({type}), typeof({type}).GetCustomAttribute<Damdor.Statio.StatioParameterAttribute>().Name);");
            });
        }

        
        
    }
}