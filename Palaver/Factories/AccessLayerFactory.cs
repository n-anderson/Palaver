using System;
using Palaver.Attributes;

namespace Palaver.Factories
{
    /// <summary>
    /// Depending on mode, this factory can produce real db access class instances or mock objects.
    /// It uses custom attributes to determine which is which.
    /// </summary>
    /// <typeparam name="SC">The specific type of context to be used by the db access classes.</typeparam>
    public class AccessLayerFactory<SC>
    {
        #region CreateObjectMode enum

        public enum CreateObjectMode
        {
            Natural,
            Mock
        }

        #endregion

        public AccessLayerFactory(CreateObjectMode m)
        {
            Mode = m;
        }

        public CreateObjectMode Mode { get; set; }

        /// <summary>
        /// Generic method constructs an object based on the specified interface.
        /// If the factory mode is "natural" a normal db access object will be constructed.
        /// If the factory mode is "mock" a mock db access object will be constructed.
        /// </summary>
        /// <typeparam name="I">The interface for the needed access class.</typeparam>
        /// <param name="Context">The context the access class should operate on.</param>
        /// <returns></returns>
        public I GetAccessLayerClass<I>(SC Context)
        {
            if (Mode == CreateObjectMode.Natural)
            {
                foreach (Type A in typeof (I).Assembly.GetTypes())
                {
                    if (A.GetInterface(typeof (I).FullName) != null &&
                        A.GetCustomAttributes(typeof (NaturalObject), false).Length == 1)
                    {
                        return (I) Activator.CreateInstance(A, Context);
                    }
                }
                throw new ArgumentException("No compatible access object found for " + typeof (I).FullName, "I");
            }
            else
            {
                foreach (Type A in typeof (I).Assembly.GetTypes())
                {
                    if (A.GetInterface(typeof (I).FullName) != null &&
                        A.GetCustomAttributes(typeof (MockObject), false).Length == 1)
                    {
                        return (I) Activator.CreateInstance(A);
                    }
                }
                throw new ArgumentException("No compatible mock object found for " + typeof (I).FullName, "I");
            }
        }
    }
}