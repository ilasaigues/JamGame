using System;
using UnityEngine;

public static class StateBehaviour
{
    public static float CalculateHorizontalVelocity(float current, float targetVelocity, float acceleration, float deceleration, float delta)
    {
        float chosenAcceleration;

        if (targetVelocity != 0) // moving, accelerate to target speed
        {
            chosenAcceleration = acceleration;
        }
        else // not moving, decelerate to zero
        {
            chosenAcceleration = deceleration;
        }

        var diff = targetVelocity - current;

        if (Mathf.Abs(diff) > chosenAcceleration * delta) // if we won't go over the target speed, accelerate
        {
            return current + Mathf.Sign(diff) * chosenAcceleration * delta;
        }
        else // if we would go over the target speed, just set it to the target speed
        {
            return targetVelocity;
        }
    }

    public static float CalculateVerticalVelocity(float current, float maxAirVelocity, Func<float> GravityGetter, float delta)
    {
        current += GravityGetter() * delta;
        if (Mathf.Abs(current) > maxAirVelocity) // if the velocity would exceed the max, clamp it
        {
            current = maxAirVelocity * Mathf.Sign(current);
        }
        return current;
    }
}