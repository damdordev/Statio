using System;

namespace Damdor.Statio
{
    /// <summary>
    /// Marks a parameter class for registration by the Statio source generator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StatioParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the Inspector menu name, with slashes separating menu groups.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Creates a parameter registration attribute.
        /// </summary>
        /// <param name="name">The Inspector menu name.</param>
        public StatioParameterAttribute(string name)
        {
            Name = name;
        }
    }
}