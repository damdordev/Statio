using System;

namespace Damdor.Statio
{
    public class StatioParameterName : Attribute
    {
        public string Name { get; }

        public StatioParameterName(string name)
        {
            Name = name;
        }

    }
}