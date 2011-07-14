using System.Data.Linq;
using Palaver.Factories;
using Palaver.Interfaces;
using Palaver.ManagerFetchers;

namespace Palaver.LinqToSql
{
    /// <summary>
    /// Sets everything up to go through a facade called "Helpers."
    /// Passes the Facade the correct generic parameters to use Linq To SQL and persist
    /// the conversation manager in session.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultGateway<SC> : IGateway
        where SC : DataContext
    {
        public DefaultGateway()
        {
            Helpers =
                new Facade<DataContext, SC, Session<Manager, Conversation>, Manager, Conversation>(AccessLayerFactory<SC>.CreateObjectMode.Natural);
        }

        #region IGateway Members

        public IFacade Helpers { get; set; }

        #endregion
    }
}