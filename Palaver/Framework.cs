using Palaver.Base;
using Palaver.Factories;

namespace Palaver
{
    /// <summary>
    /// Defines precisely how all classes in the library interact with each other.
    /// All generic type constraints have their origin here.
    /// The facade presented to client code accesses the library through this class to ensure
    /// that everything is put together correctly and consistently.
    /// </summary>
    /// <typeparam name="BC">Base context type being used (eg. DataContext for Linq To SQL).</typeparam>
    /// <typeparam name="SC">Specific context type.</typeparam>
    /// <typeparam name="MF">Manager fetcher type.</typeparam>
    /// <typeparam name="M">Manager type.</typeparam>
    /// <typeparam name="C">Conversation type.</typeparam>
    public class Framework<BC, SC, MF, M, C>
        where MF : ManagerFetcher<M, C>, new()
        where M : Manager<C>
        where C : Conversation<BC>
        where SC : BC
    {
        public AccessLayerFactory<SC> Factory;
        public MF Fetcher;

        public Framework(AccessLayerFactory<SC>.CreateObjectMode AccessLayerFactoryMode)
        {
            Fetcher = new MF();
            Factory = new AccessLayerFactory<SC>(AccessLayerFactoryMode);
        }

        public M Manager
        {
            get { return Fetcher.Manager; }
        }
    }
}