using System.Web;
using Palaver.Base;

namespace Palaver.ManagerFetchers
{
    /// <summary>
    /// Manager fetcher designed to fetch managers from session. Whenever you get a manager, a manager fetcher should be used.
    /// </summary>
    /// <typeparam name="M">Type of manager to fetch.</typeparam>
    /// <typeparam name="C">Type of conversation the manager is managing.</typeparam>
    public class Session<M, C> : ManagerFetcher<M, C> where M : Manager<C>
    {
        public override M Manager
        {
            get { return HttpContext.Current.Session[Module.StorageKey] as M; }
            set { HttpContext.Current.Session[Module.StorageKey] = value; }
        }

        public override string scopeIdentifier
        {
            get { return HttpContext.Current.Request.Path; }
        }
    }
}