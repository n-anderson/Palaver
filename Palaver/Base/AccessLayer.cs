namespace Palaver.Base
{
    /// <summary>
    /// Base class which all data access classes should inherit from.
    /// </summary>
    /// <typeparam name="SC">Specifies the specific type of context the access class depends on.</typeparam>
    public abstract class AccessLayer<SC>
    {
        /// <summary>
        /// Context instance is passed into each access class on construction.
        /// </summary>
        /// <param name="Context">The context instance to be operated on.</param>
        public AccessLayer(SC Context)
        {
            this.Context = Context;
        }

        /// <summary>
        /// Property exposing context instance to descendants.
        /// </summary>
        protected SC Context { get; set; }
    }
}