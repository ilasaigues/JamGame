using AstralCore;
using UnityEngine;

public class BaseHazard : TimeboundMonoBehaviour
{
    [SerializeField]
    public PowerPickupSpawner powerPickupSpawner;
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

    public void SpawnPickup(Vector2 position)
    {
        powerPickupSpawner.SpawnPickup(PowerupConfig, position);
    }

    void OnDrawGizmosSelected()
    {
        if (powerPickupSpawner != null)
        {
            Gizmos.DrawLine(transform.position, powerPickupSpawner.transform.position);
        }
    }

}
