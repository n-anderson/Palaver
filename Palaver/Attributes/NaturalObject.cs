using System;

namespace Palaver.Attributes
{
    /// <summary>
    /// Marks a data access class as not being a mock object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NaturalObject : Attribute
    {
    }
}