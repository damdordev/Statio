using Microsoft.CodeAnalysis;

namespace Damdor.Statio.CodeGenerator
{
    [Generator]
    public class StatioSettingsCodeGenerator : ClassRegisterViaAttributeCodeGenerator
    {
        public const string StatioParameterAttribute = "StatioParameter";

        public override string[] Attributes { get; } = { StatioParameterAttribute };

        public override void Execute(GeneratorExecutionContext context)
        {
            GenerateForAttribute(context, StatioParameterAttribute, (sb, type) =>
            {
                sb.AppendLine($"        global::Damdor.Statio.StatioSettings.RegisterParameterType(typeof({type}), typeof({type}).GetCustomAttribute<Damdor.Statio.StatioParameterAttribute>().Name);");
            });
        }

        
        
    }
}