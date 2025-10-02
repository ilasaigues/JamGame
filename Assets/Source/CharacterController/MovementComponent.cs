using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AstralCore;
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovementComponent : TimeboundMonoBehaviour
{

    public bool IsAgainstFloor { get; set; }
    public bool IsAgainstWall { get; set; }
    public bool IsAgainstCeiling { get; set; }

    private readonly List<Collision2D> CeilingCollisions = new();
    private readonly List<Collision2D> GroundCollisions = new();
    private readonly List<Collision2D> WallCollisions = new();

    public enum VelocityType
    {
        MainMovement,
        Gravity,
        InheritedVelocity,
        Dash,
    }
    private readonly Dictionary<VelocityType, Vector2> Velocities = new();

    public Vector2 CurrentVelocity => Velocities.Values.AsEnumerable()
        .Append(Vector2.zero)
        .Aggregate((v1, v2) => v1 + v2);


    public Vector2 GetVelocity(VelocityType type)
    {
        return Velocities.ContainsKey(type) ? Velocities[type] : Vector2.zero;
    }
    public void SetVelocity(VelocityType type, Vector2 velocity)
    {
        Velocities[type] = velocity;
    }

    public void Awake()
    {
        Velocities.Add(VelocityType.MainMovement, Vector2.zero);
        Velocities.Add(VelocityType.Gravity, Vector2.zero);
        Velocities.Add(VelocityType.InheritedVelocity, Vector2.zero);
        Velocities.Add(VelocityType.Dash, Vector2.zero);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contactPoints = { };
        collision.GetContacts(contactPoints);
        foreach (var contactPoint in contactPoints)
        {
            if (contactPoint.normal.y > 0.5f) // ground touch
            {
                GroundCollisions.Add(collision);
            }
            else if (contactPoint.normal.y < -0.5f) // ceiling touch
            {
                CeilingCollisions.Add(collision);
            }
            else // assume wall touch
            {
                WallCollisions.Add(collision);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ContactPoint2D[] contactPoints = { };
        collision.GetContacts(contactPoints);
        foreach (var contactPoint in contactPoints)
        {
            if (contactPoint.normal.y > 0.5f) // ground touch
            {
                GroundCollisions.Remove(collision);
            }
            else if (contactPoint.normal.y < -0.5f) // ceiling touch
            {
                CeilingCollisions.Remove(collision);
            }
            else // assume wall touch
            {
                WallCollisions.Remove(collision);
            }
        }
    }

    public void PhysicsMove(Rigidbody2D rb)
    {
        rb.linearVelocity = CurrentVelocity;
    }

}
