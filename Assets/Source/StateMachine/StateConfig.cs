using UnityEngine;

public partial class StateConfig
{
    public interface IBaseStateConfig { }
    public class StartingVelocityConfig : IBaseStateConfig
    {
        public Vector2 Velocity;

        public StartingVelocityConfig(Vector2 velocity)
        {
            Velocity = velocity;
        }
    }
}