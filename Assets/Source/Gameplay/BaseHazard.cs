using UnityEngine;

public class BaseHazard : MonoBehaviour
{
    [SerializeField]
    private PowerPickupSpawner powerPickupSpawner;
    public PowerupConfig PowerupConfig;

    void OnTriggerEnter2D(Collider2D collision)
    {
        var components = collision.GetComponents<MonoBehaviour>();
        foreach (var component in components)
        {
            if (component is IKillable killableComponent)
            {
                OnContactWithKillable(killableComponent);
            }
        }
    }

    protected virtual void OnContactWithKillable(IKillable killable)
    {
        if (killable.CanBeKilledBy(this))
        {
            StartCoroutine(killable.Kill(this));
        }
    }

    public void SpawnPickup()
    {
        powerPickupSpawner.SpawnPickup(PowerupConfig);
    }

    void OnDrawGizmosSelected()
    {
        if (powerPickupSpawner != null)
        {
            Gizmos.DrawLine(transform.position, powerPickupSpawner.transform.position);
        }
    }

}
