namespace Palaver.Base
{
    /// <summary>
    /// ManagerFetcher descendants specify where the conversation manager should be stored. This might be Session or thread scope, etc.
    /// </summary>
    /// <typeparam name="M">The type of Manager to be stored.</typeparam>
    /// <typeparam name="C">The type of Conversation the Manager will manage.</typeparam>
    public abstract class ManagerFetcher<M, C> where M : Manager<C>
    {
        public abstract M Manager { get; set; }
        public abstract string scopeIdentifier { get; }
    }
}