using System;

namespace Palaver.Attributes
{
    /// <summary>
    /// Marks a data access class as a mock object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MockObject : Attribute
    {
    }
}