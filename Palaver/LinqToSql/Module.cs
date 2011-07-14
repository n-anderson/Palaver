using System;
using System.Web;
using Palaver.Interfaces;

namespace Palaver.LinqToSql
{
    public class Module : Base.Module
    {
        /// <summary>
        /// To be called when session ends. This will make sure every conversation is disposed of.
        /// </summary>
        public static void CancelAllConversationsInSession()
        {
            var dcm = HttpContext.Current.Session[StorageKey] as Manager;
            dcm.CancelAllConversations();
        }

        /// <summary>
        /// Look for a Manager. If one is not found, create a new one. CancelAllOutOfScopeConversations()
        /// as this marks the beginning of a new request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //We really only want to deal with page requests
            var app = (HttpApplication) sender;
            if (app.Context.Handler is IPalaverPage)
            {
                var dcm = app.Context.Session[StorageKey] as Manager;
                if (dcm == null)
                {
                    dcm = new Manager();
                    app.Context.Session[StorageKey] = dcm;
                }
                dcm.CancelAllOutOfScopeConversations(app.Context.Request.Path);
            }
        }

        /// <summary>
        /// At this point there should always be a Manager in Session.
        /// Get it and call CancelAllNonPersistentConversations() as this marks the end of the request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            //We really only want to deal with page requests
            var app = (HttpApplication) sender;
            if (app.Context.Handler is IPalaverPage)
            {
                var dcm = app.Context.Session[StorageKey] as Manager;
                dcm.CancelAllNonPersistentConversations();
            }
        }
    }
}