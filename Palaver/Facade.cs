using System;
using System.Collections.Generic;
using Palaver.Base;
using Palaver.Factories;

namespace Palaver
{
    /// <summary>
    /// This class presents the external interface of the liobrary to client code. All calls to the library go through here.
    /// The point is to simplify interaction with the library.
    /// </summary>
    /// <typeparam name="BC">Base context type being used (eg. DataContext for Linq To SQL).</typeparam>
    /// <typeparam name="SC">Specific context type.</typeparam>
    /// <typeparam name="MF">Manager fetcher type.</typeparam>
    /// <typeparam name="M">Manager type.</typeparam>
    /// <typeparam name="C">Conversation type.</typeparam>
    public class Facade<BC, SC, MF, M, C> : IFacade
        where MF : ManagerFetcher<M, C>, new()
        where M : Manager<C>
        where C : Conversation<BC>
        where SC : BC
    {
        public Facade(AccessLayerFactory<SC>.CreateObjectMode AccessLayerFactoryMode)
        {
            Framework = new Framework<BC, SC, MF, M, C>(AccessLayerFactoryMode);
        }

        public Framework<BC, SC, MF, M, C> Framework { get; set; }

        #region IFacade Members

        public void BeginConversation()
        {
            Framework.Manager.BeginDefaultConversation(typeof(SC), new List<string> {Framework.Fetcher.scopeIdentifier}, false);
        }

        public void BeginConversation(bool persist)
        {
            Framework.Manager.BeginDefaultConversation(typeof(SC), new List<string> {Framework.Fetcher.scopeIdentifier}, persist);
        }

        public string BeginConversationUnique()
        {
            return Framework.Manager.BeginNamedConversation(typeof(SC), new List<string> {Framework.Fetcher.scopeIdentifier}, false);
        }

        public string BeginConversationUnique(bool persist)
        {
            return Framework.Manager.BeginNamedConversation(typeof(SC), new List<string> {Framework.Fetcher.scopeIdentifier}, persist);
        }

        public void CommitConversation()
        {
            Framework.Manager.CommitDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, false);
        }

        public void CommitConversation(bool persist)
        {
            Framework.Manager.CommitDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, persist);
        }

        public void CommitConversation(string ConversationId)
        {
            Framework.Manager.CommitNamedConversation(ConversationId);
        }

        public void CancelConversation()
        {
            Framework.Manager.CancelDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, false);
        }

        public void CancelConversation(bool persist)
        {
            Framework.Manager.CancelDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, persist);
        }

        public void CancelConversation(string ConversationId)
        {
            Framework.Manager.CancelNamedConversation(ConversationId);
        }

        public AC GetAccessClass<AC>()
        {
            if(Framework.Manager.GetDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, false) == null)
            {
                BeginConversation();
            }
            return
                Framework.Factory.GetAccessLayerClass<AC>(
                    (SC) Framework.Manager.GetDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, false).Context);
        }

        public AC GetAccessClass<AC>(bool persist)
        {
            if(Framework.Manager.GetDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, persist) ==
               null)
            {
                BeginConversation(persist);
            }
            return
                Framework.Factory.GetAccessLayerClass<AC>(
                    (SC)
                    Framework.Manager.GetDefaultConversation(typeof(SC), Framework.Fetcher.scopeIdentifier, persist).Context);
        }

        public AC GetAccessClass<AC>(string ConversationId)
        {
            C Conversation = Framework.Manager.GetNamedConversation(ConversationId);
            if(Conversation == null)
            {
                throw new ArgumentException("Conversation with given ID not found", "ConversationId");
            }
            return Framework.Factory.GetAccessLayerClass<AC>((SC) Conversation.Context);
        }

        #endregion
    }

    /// <summary>
    /// Used only to allow for a common interface among all the different possible types
    /// of Facade instances with different generic parameters.
    /// </summary>
    public interface IFacade
    {
        void BeginConversation();

        void BeginConversation(bool persist);

        string BeginConversationUnique();

        string BeginConversationUnique(bool persist);

        void CommitConversation();
        void CommitConversation(bool persist);
        void CommitConversation(string ConversationId);
        void CancelConversation();
        void CancelConversation(bool persist);
        void CancelConversation(string ConversationId);

        AC GetAccessClass<AC>();

        AC GetAccessClass<AC>(bool persist);

        AC GetAccessClass<AC>(string ConversationId);
    }
}