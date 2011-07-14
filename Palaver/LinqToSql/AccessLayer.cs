using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Dynamic;
using Palaver.Base;
using Palaver.Interfaces;

namespace Palaver.LinqToSql
{
    public abstract class AccessLayer<SC, E, PK> : AccessLayer<SC>, IAccessLayer<E, PK> where SC : DataContext
                                                                                        where E : class
    {
        protected AccessLayer(SC Context, Table<E> EntityTable, string PrivateKeyName) : base(Context)
        {
            this.EntityTable = EntityTable;
            this.PrivateKeyName = PrivateKeyName;
        }

        private Table<E> EntityTable { get; set; }
        private string PrivateKeyName { get; set; }

        #region IAccessLayer<E,PK> Members

        public virtual List<E> getAll()
        {
            return (from e in EntityTable
                    select e).ToList();
        }

        public virtual E getById(PK id)
        {
            return EntityTable.Where("@0 = @1", PrivateKeyName, id).FirstOrDefault();
        }

        public virtual void Insert(E record)
        {
            EntityTable.InsertOnSubmit(record);
        }

        public virtual void Delete(E record)
        {
            EntityTable.DeleteOnSubmit(record);
        }

        #endregion
    }
}