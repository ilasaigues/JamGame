using UnityEngine;

public class PowerPickupSpawner : MonoBehaviour
{
    [SerializeField] protected PowerPickup PowerPickupPrefab;

    public void SpawnPickup(PowerupConfig config)
    {
        var pickup = Instantiate(PowerPickupPrefab);
        pickup.transform.position = transform.position;
        pickup.powerupType = config.PowerupType;
        pickup.GetComponent<SpriteRenderer>().sprite = config.PickupSprite;
    }
}
