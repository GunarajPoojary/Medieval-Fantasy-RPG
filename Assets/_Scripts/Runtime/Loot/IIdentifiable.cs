namespace RPG.World
{
    /// <summary>
    /// Interface for objects that have a unique identifier.
    /// </summary>
    public interface IIdentifiable
    {
        string Id { get; }
        void GenerateGuid();
    }
}