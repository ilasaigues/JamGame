using System.Collections;

public interface IKillable
{
    bool CanBeKilledBy(BaseHazard hazard);
    IEnumerator Kill(BaseHazard hazard);
}
