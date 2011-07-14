using System;
using System.Collections.Generic;

namespace Palaver.Base
{
    /// <summary>
    /// Every concurrent conversation is tracked and managed by descendants of this class. This class is not designed to be thread safe,
    /// so access to it should only happen synchronously. ASP.NET protects items stored in session from concurrent requests by queuing
    /// requests that happen on the same session. (Unless you tell it not to.)
    /// </summary>
    /// <typeparam name="C">The type of Conversation that will be managed.</typeparam>
    public abstract class Manager<C>
    {
        protected Manager()
        {
            Conversations = new List<C>();
        }

        /// <summary>
        /// List of ongoing conversations.
        /// </summary>
        protected List<C> Conversations { get; set; }

        /// <summary>
        /// Begins a default conversation. Parameters uniquely identify the conversation.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="scopeIdentifiers"></param>
        /// <param name="isPersistent"></param>
        public abstract void BeginDefaultConversation(Type dcType, List<string> scopeIdentifiers, bool isPersistent);

        /// <summary>
        /// Retrieves a default conversation. Parameters uniquely identify the conversation.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="scopeIdentifier"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public abstract C GetDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent);

        /// <summary>
        /// Begins a named conversation and returns the name of the conversation.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="scopeIdentifiers"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public abstract string BeginNamedConversation(Type dcType, List<string> scopeIdentifiers, bool isPersistent);

        /// <summary>
        /// Retrieves a named conversation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract C GetNamedConversation(string name);

        /// <summary>
        /// Commits the default conversation specified by the parameters.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="scopeIdentifier"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public abstract bool CommitDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent);

        /// <summary>
        /// Cancels the default conversation specified by the parameters.
        /// </summary>
        /// <param name="dcType"></param>
        /// <param name="scopeIdentifier"></param>
        /// <param name="isPersistent"></param>
        /// <returns></returns>
        public abstract bool CancelDefaultConversation(Type dcType, string scopeIdentifier, bool isPersistent);

        /// <summary>
        /// Commits a named conversation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract bool CommitNamedConversation(string name);

        /// <summary>
        /// Cancels a named conversation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract bool CancelNamedConversation(string name);
    }
}