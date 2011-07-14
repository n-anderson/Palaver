using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using Palaver.Base;

namespace Palaver.LinqToSql
{
    /// <summary>
    /// Conversation specifically for Linq To SQL
    /// </summary>
    public class Conversation : Conversation<DataContext>
    {
        public Conversation(DataContext context, bool persistent, List<string> scopeIdentifiers, bool isDefault)
            : base(context, persistent, scopeIdentifiers, isDefault)
        {
        }

        /// <summary>
        /// Linq To SQL automatically performs optimistic concurrency checks. For most cases,
        /// it should be ok to override this functionality by resolving all change conflicts to
        /// "keep current values". Bascially, this means that if there is a change conflict, first
        /// in wins.
        /// 
        /// TODO: This should be extended in the future to have several modes for handling concurrency
        /// conflicts rather than just defaulting everything to first in wins.
        /// </summary>
        public override void Commit()
        {
            try
            {
                Context.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                //TO DO: conditionally log exception
                foreach (ObjectChangeConflict occ in Context.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                }
            }

            //If a transaction is ongoing, commit it
            if (Context.Transaction != null)
            {
                Context.Transaction.Commit();
            }

            //Dispose of datacontext and stop persisting
            Context.Dispose();
            if (onCancelOrCommit != null) onCancelOrCommit(this);
        }

        /// <summary>
        /// Dispose of context and fire event.
        /// </summary>
        public override void Cancel()
        {
            //If a transaction is ongoing, roll it back
            if (Context.Transaction != null)
            {
                Context.Transaction.Rollback();
            }

            Context.Dispose();
            if (onCancelOrCommit != null) onCancelOrCommit(this);
        }

        public override event Action<Conversation<DataContext>> onCancelOrCommit;

        public override void BeginTransaction()
        {
            Context.Connection.Open();
            DbTransaction t = Context.Connection.BeginTransaction();
            Context.Transaction = t;
        }

        public override void CommitTransaction()
        {
            Context.Transaction.Commit();
        }

        public override void RollbackTransaction()
        {
            Context.Transaction.Rollback();
        }
    }
}