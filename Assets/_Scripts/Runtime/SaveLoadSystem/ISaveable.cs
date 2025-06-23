namespace ProjectEmbersteel
{
    public interface ISaveable
    {
        string ID { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        string ID { get; set; }
        void Bind(TData data);
    }
}