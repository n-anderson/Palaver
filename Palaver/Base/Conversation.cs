using System;
using System.Collections.Generic;

namespace Palaver.Base
{
    /// <summary>
    /// Abstract representation of a conversation from which every conversation should descend.
    /// </summary>
    /// <typeparam name="C">
    /// The base type of context being wrapped by each conversation. 
    /// (For LINQ to SQL, this is a DataContext. For NHibernate, it would be a Session)
    /// </typeparam>
    public abstract class Conversation<C>
    {
        /// <summary>
        /// At construction, every conversation needs these parameters.
        /// </summary>
        /// <param name="context">Context to wrap.</param>
        /// <param name="isPersistent">Flag indicating whether the conversation should persist beyond the timeframe specified by the manager.</param>
        /// <param name="scopeIdentifiers">Collection of scope identifiers (could be urls) indicating where a conversation is valid.</param>
        /// <param name="isDefault">
        /// Flag indicating whether the conversation is "default" or not. There can be at most 2 default conversations per individual scope 
        /// identifier: a persistent one and a non-persistent one. The whole concept of default conversations is just for convenience in using the library.
        /// </param>
        public Conversation(C context, bool isPersistent, List<string> scopeIdentifiers, bool isDefault)
        {
            Context = context;
            this.isPersistent = isPersistent;
            ScopeIdentifiers = scopeIdentifiers;
            this.isDefault = isDefault;
            Name = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Context to wrap.
        /// </summary>
        public C Context { get; private set; }

        /// <summary>
        /// Flag indicating whether the conversation should persist beyond the timeframe specified by the manager.
        /// </summary>
        public bool isPersistent { get; protected set; }

        /// <summary>
        /// Flag indicating whether the conversation is "default" or not. There can be at most 2 default conversations per individual scope 
        /// identifier: a persistent one and a non-persistent one. The whole concept of default conversations is just for convenience in using the library.
        /// </summary>
        public bool isDefault { get; protected set; }

        /// <summary>
        /// Collection of scope identifiers (could be urls) indicating where a conversation is valid.
        /// </summary>
        public List<string> ScopeIdentifiers { get; private set; }

        /// <summary>
        /// The name should represent a unique identifier for the conversation. Useful in situations where
        /// the two default conversations just won't cut it.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// This event should be fired whenever a conversation is cancelled or committed.
        /// Managers can subscribe to be notified when a conversation ends.
        /// </summary>
        public abstract event Action<Conversation<C>> onCancelOrCommit;

        /// <summary>
        /// Every conversation needs to be able to persist changes back to the db.
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Every conversation needs to be able to abort current changes and dispose of the context.
        /// </summary>
        public abstract void Cancel();

        /// <summary>
        /// Begin a transaction
        /// </summary>
        public abstract void BeginTransaction();

        /// <summary>
        /// Commit the ongoing transaction
        /// </summary>
        public abstract void CommitTransaction();

        /// <summary>
        /// Rollback the ongoing transaction
        /// </summary>
        public abstract void RollbackTransaction();
    }
}