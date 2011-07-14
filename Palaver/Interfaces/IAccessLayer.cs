using System.Collections.Generic;

namespace Palaver.Interfaces
{
    /// <summary>
    /// Every db access class should implement this interface.
    /// </summary>
    /// <typeparam name="E">The type of entity being queried.</typeparam>
    /// <typeparam name="PK">The type of the primary key for the entity.</typeparam>
    public interface IAccessLayer<E, PK>
    {
        List<E> getAll();
        E getById(PK id);
        void Insert(E record);
        void Delete(E record);
    }
}