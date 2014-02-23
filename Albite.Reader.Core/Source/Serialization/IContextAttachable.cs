namespace Albite.Reader.Core.Serialization
{
    /// <summary>
    /// Represents an entity that is only usuable when being attached to a context.
    ///
    /// This is particularly useful for entities that need to be serialized, yet
    /// can't be fully serialized because some of their members' lifetime spans
    /// beyond the lifetime of the entity.
    /// </summary>
    /// <typeparam name="TContext">The cotext type to attach to</typeparam>
    public interface IContextAttachable<TContext>
    {
        TContext Context { get; }
        bool IsAttached { get; }
        void Attach(TContext context);
    }
}
