using System;
using System.Web;

namespace Palaver.Base
{
    /// <summary>
    /// In ASP.NET applications, descendants of this module manage persistence of the Conversation Manager object in session.
    /// The HttpModule also may call Manager methods that need to be called for cleanup before and after each request. 
    /// </summary>
    public abstract class Module : IHttpModule
    {
        public static string _StorageKey = String.Empty;

        /// <summary>
        /// A unique storage key.
        /// </summary>
        public static string StorageKey
        {
            get
            {
                if (_StorageKey == String.Empty)
                {
                    _StorageKey = Guid.NewGuid().ToString();
                }
                return _StorageKey;
            }
        }

        #region IHttpModule Members

        /// <summary>
        /// Every HttpModule must implement Dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Ties up events to be fired before and after the execution of the request handler (Page).
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;
            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }

        #endregion

        protected abstract void context_PreRequestHandlerExecute(object sender, EventArgs e);

        protected abstract void context_PostRequestHandlerExecute(object sender, EventArgs e);
    }
}