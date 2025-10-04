using System.Collections;
using UnityEngine;

public interface IKillable
{
    bool CanBeKilledBy(BaseHazard hazard);
    IEnumerator Kill(BaseHazard hazard);
}
