namespace AstralCore
{
    public interface ITimeContext
    {
        public float DeltaTime { get; }
        public float FixedDeltaTime { get; }
    }
}
