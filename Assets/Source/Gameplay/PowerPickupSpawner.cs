using DG.Tweening;
using UnityEngine;

public class PowerPickupSpawner : MonoBehaviour
{
    [SerializeField] protected PowerPickup PowerPickupPrefab;

    private PowerPickup _powerPickupInstance;

    public void SpawnPickup(PowerupConfig config, Vector2 position)
    {
        var pickup = Instantiate(PowerPickupPrefab);
        pickup.transform.position = position;
        pickup.powerupType = config.PowerupType;
        pickup.GetComponent<Animator>().SetFloat("PowerupType", (int)config.PowerupType);
        MovePowerupToPosition(pickup);
    }

    public void MovePowerupToPosition(PowerPickup newPickup)
    {
        newPickup.transform.DOMove(transform.position, 0.5f).OnComplete(() =>
        {
            if (_powerPickupInstance != null)
            {
                Destroy(newPickup.gameObject);
                _powerPickupInstance.AddPowerupCount();
            }
            else
            {
                _powerPickupInstance = newPickup;
                _powerPickupInstance.GetComponent<Animator>().SetTrigger("Transform");
            }
            _powerPickupInstance?.UpdateCountDisplay();
        });
    }
}
