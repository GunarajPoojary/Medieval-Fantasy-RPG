namespace ProjectEmbersteel.StatSystem
{
    public interface IStatModifiable
    {
        void AddStatModifier(StatModifier modifier);
        void RemoveStatModifier(StatModifier modifier);
    }
}