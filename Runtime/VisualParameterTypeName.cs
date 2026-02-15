using System;

namespace Damdor.VisualStates
{
    public class VisualParameterTypeName : Attribute
    {
        public string Name { get; }

        public VisualParameterTypeName(string name)
        {
            Name = name;
        }

    }
}