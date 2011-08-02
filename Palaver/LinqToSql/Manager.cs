using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Palaver.Base;

namespace Palaver.LinqToSql
{
    /// <summary>
    /// Manager intended to handle instances of Linq To SQL conversations.
    /// </summary>
    public class Manager : Manager<Conversation>
    {
        /// <summary>
        /// Begins a default conversation. The parameters uniquely identify the conversation.
        /// </summary>
        /// <param name="dcType">Specific type of the datacontext.</param>
        /// <param name="scopeIdentifiers">Collection of valid scope identifiers for the conversation.</param>
        /// <param name="isPersistent">Flag indicating whether or not conversation persists beyond one request.</param>
        public override void BeginDefaultConversation(Type dcType, List<string> scopeIdentifiers, bool isPersistent)
        {
            foreach(string id in scopeIdentifiers)
            {
                if(FindDefaultConversation(dcType, isPersistent, id) != null)
                {
                    return;
                }
            }
            CreateConversation(dcType, isPersistent, scopeIdentifiers, true);
        }

        /// <summary>
        /// Returns a default conversation identified by the parameters.
        /// </summary>
        /// <param name="dcType">Specific type of the datacontext.</param>
        /// <param name="scopeIdentifiers">Collection of valid scope identifiers for the conversation.</param>
        /// <param name="isPersistent">Flag indicating whether or not conversation persists beyond one request.</param>
        /// <returns>Linq To SQL conversation matching the parameters.</returns>
        public override Conversation GetDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent)
        {
            return FindDefaultConversation(dcType, isPersistent, scopeIdentifier);
        }

        /// <summary>
        /// Begins a named conversation and sets it up according to the parameters passed.
        /// </summary>
        /// <param name="dcType">Specific type of the datacontext.</param>
        /// <param name="scopeIdentifiers">Collection of valid scope identifiers for the conversation.</param>
        /// <param name="isPersistent">Flag indicating whether or not conversation persists beyond one request.</param>
        /// <returns>The unique name given to the conversation on creation.</returns>
        public override string BeginNamedConversation(Type dcType, List<string> scopeIdentifiers, bool isPersistent)
        {
            return CreateConversation(dcType, isPersistent, scopeIdentifiers, false).Name;
        }

        /// <summary>
        /// Returns a named conversation having the name indicated by the parameter.
        /// </summary>
        /// <param name="name">Unique name of the conversation.</param>
        /// <returns>Linq To SQL conversation with the unique name indicated by the parameter.</returns>
        public override Conversation GetNamedConversation(string name)
        {
            return FindNamedConversation(name);
        }

        /// <summary>
        /// Commits the default conversation indicated by the parameters passed.
        /// </summary>
        /// <param name="dcType">Specific type of the datacontext.</param>
        /// <param name="scopeIdentifiers">Collection of valid scope identifiers for the conversation.</param>
        /// <param name="isPersistent">Flag indicating whether or not conversation persists beyond one request.</param>
        /// <returns>Flag indicating success or failure of the commit.</returns>
        public override void CommitDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent)
        {
            FindDefaultConversation(dcType, isPersistent, scopeIdentifier).Commit();
        }

        /// <summary>
        /// Cancels the default conversation indicated by the parameters passed.
        /// </summary>
        /// <param name="dcType">Specific type of the datacontext.</param>
        /// <param name="scopeIdentifier">Collection of valid scope identifiers for the conversation.</param>
        /// <param name="isPersistent">Flag indicating whether or not conversation persists beyond one request.</param>
        /// <returns>Flag indicating success or failure of the cancel operation.</returns>
        public override void CancelDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent)
        {
            Conversation DefaultConversation = FindDefaultConversation(dcType, isPersistent, scopeIdentifier);
            if(DefaultConversation != null)
            {
                DefaultConversation.Cancel();
            }
        }

        /// <summary>
        /// Commits a named conversation.
        /// </summary>
        /// <param name="name">Unique name of the conversation to be committed.</param>
        /// <returns>Flag indicating success or failure of the commit.</returns>
        public override void CommitNamedConversation(string name)
        {
            FindNamedConversation(name).Commit();
        }

        /// <summary>
        /// Cancels a named conversation.
        /// </summary>
        /// <param name="name">Unique name of the conversation to be cancelled.</param>
        /// <returns>Flag indicating success or failure of the cancel operation.</returns>
        public override void CancelNamedConversation(string name)
        {
            FindNamedConversation(name).Cancel();
        }

        /// <summary>
        /// Method used internally to create each conversation according to the parameters passed.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="isPersistent"></param>
        /// <param name="scopeIdentifiers"></param>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        private Conversation CreateConversation(Type dcType, bool isPersistent, List<string> scopeIdentifiers,
                                                bool isDefault)
        {
            if(dcType == null)
            {
                throw new ArgumentNullException("dcType", "dcType cannot be null");
            }
            if(!dcType.IsSubclassOf(typeof(DataContext)))
            {
                throw new ArgumentException("dcType must inherit from System.Data.Linq.DataContext", "dcType");
            }
            if(scopeIdentifiers == null || scopeIdentifiers.Count <= 0)
            {
                throw new ArgumentException("scopeIdentifiers must contain at least one string", "scopeIdentifiers");
            }

            try
            {
                var dc = (DataContext) Activator.CreateInstance(dcType);
                var cc = new Conversation(dc, isPersistent, scopeIdentifiers, isDefault);
                cc.onCancelOrCommit += RemoveConversation;
                Conversations.Add(cc);
                return cc;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Method used internally to locate default conversations.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="isPersistent"></param>
        /// <param name="scopeIdentifier"></param>
        /// <returns></returns>
        private Conversation FindDefaultConversation(Type dcType, bool isPersistent, string scopeIdentifier)
        {
            if(dcType == null)
            {
                throw new ArgumentNullException("dcType", "dcType cannot be null");
            }
            if(!dcType.IsSubclassOf(typeof(DataContext)))
            {
                throw new ArgumentException("dcType must inherit from System.Data.Linq.DataContext", "dcType");
            }
            if(String.IsNullOrEmpty(scopeIdentifier))
            {
                throw new ArgumentNullException("scopeIdentifier", "scopeIdentifier must contain at least one string");
            }

            List<Conversation> c;
            c =
                Conversations.FindAll(
                    cc =>
                    cc.Context.GetType() == dcType && cc.isDefault && cc.isPersistent == isPersistent &&
                    cc.ScopeIdentifiers.Contains(scopeIdentifier));

            if(c.Count > 1)
            {
                throw new Exception("Duplicate default conversations found");
            }

            return c.FirstOrDefault();
        }

        /// <summary>
        /// Method used internally to locate named conversations.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Conversation FindNamedConversation(string name)
        {
            if(String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "name cannot be null or empty");
            }
            List<Conversation> c;
            c = Conversations.FindAll(cc => cc.Name == name);

            if(c.Count > 1)
            {
                throw new Exception("Duplicate named conversations found");
            }

            return c.FirstOrDefault();
        }

        /// <summary>
        /// This method gets subscribed to every conversation's onCancelOrCommit event.
        /// This method removes conversations from the manager.
        /// </summary>
        /// <param name="conversation"></param>
        private void RemoveConversation(Conversation<DataContext> conversation)
        {
            Conversations.Remove((Conversation) conversation);
        }

        /// <summary>
        /// Calls Cancel() on all conversation with persist == false
        /// </summary>
        internal void CancelAllNonPersistentConversations()
        {
            for(int i = Conversations.Count - 1; i >= 0; i--)
            {
                if(!Conversations[i].isPersistent) Conversations[i].Cancel();
            }
        }

        /// <summary>
        /// Calls Cancel() on all conversations whose set of scope identifiers no longer contains a relevant string.
        /// </summary>
        /// <param name="currentScopeIdentifier"></param>
        internal void CancelAllOutOfScopeConversations(string currentScopeIdentifier)
        {
            for(int i = Conversations.Count - 1; i >= 0; i--)
            {
                if(!Conversations[i].ScopeIdentifiers.Contains(currentScopeIdentifier)) Conversations[i].Cancel();
            }
        }

        /// <summary>
        /// Calls Cancel() on all conversations contained within the manager.
        /// </summary>
        internal void CancelAllConversations()
        {
            for(int i = Conversations.Count - 1; i >= 0; i--)
            {
                Conversations[i].Cancel();
            }
        }
    }
}