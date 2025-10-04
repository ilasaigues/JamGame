using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PowerPickup : MonoBehaviour
{
    public CharacterController2d.PowerupType powerupType;

    void OnTriggerEnter2D(Collider2D collider)
    {
        var controller = collider.GetComponent<CharacterController2d>();
        if (controller != null)
        {
            controller.SetCurrentPowerup(powerupType);
            Destroy(gameObject);
        }
    }
}
