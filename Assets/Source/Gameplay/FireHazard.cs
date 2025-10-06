using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FireHazard : BaseHazard
{
    public float OnTime = 2;
    public float OffTime = 2;

    private Animator animator;


    private float remainingTime;
    private bool FireOn = false;


    private void Start()
    {
        animator = GetComponent<Animator>();
        remainingTime = OffTime;
    }

    private void Update()
    {
        remainingTime -= DeltaTime;
        if (remainingTime <= 0)
        {
            FireOn = !FireOn;
            if (FireOn)
            {
                remainingTime = OnTime;
            }
            else
            {
                remainingTime = OffTime;
            }
            animator.SetBool("FlameOn", FireOn);
        }
    }


}
