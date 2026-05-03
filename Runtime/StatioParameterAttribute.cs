using System;

namespace Damdor.Statio
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StatioParameterAttribute : Attribute
    {
        public string Name { get; }
        
        public StatioParameterAttribute(string name)
        {
            Name = name;
        }
    }
}