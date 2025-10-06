using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PowerPickup : MonoBehaviour
{
    public CharacterController2d.PowerupType powerupType;

    [SerializeField] private GameObject CounterContainer;

    private int powerupAmount = 1;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<CharacterController2d>(out var controller))
        {
            if (controller.CanTakePowerup(this))
            {
                BGMHandler.Instance.PlaySFX(controller.soundEffects.powerup);
                controller.SetCurrentPowerup(powerupType);
                powerupAmount--;
                UpdateCountDisplay();
                if (powerupAmount == 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void AddPowerupCount()
    {
        powerupAmount = Mathf.Clamp(powerupAmount + 1, 0, 3);
        UpdateCountDisplay();
    }

    public void UpdateCountDisplay()
    {
        CounterContainer.SetActive(true);
        var children = CounterContainer.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < children.Length; i++)
        {
            var counter = children[i].gameObject;
            counter.SetActive(i < powerupAmount);
            var pos = new Vector2(i / 2f - ((powerupAmount - 1) / 4f), 0);
            counter.transform.localPosition = pos;
        }
    }
}
