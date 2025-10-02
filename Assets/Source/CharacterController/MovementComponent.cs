using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AstralCore;
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MovementComponent : TimeboundMonoBehaviour
{

    public bool IsAgainstGround => GroundCollisions.Count > 0;
    public bool IsAgainstWall => WallCollisions.Count > 0;
    public bool IsAgainstCeiling => CeilingCollisions.Count > 0;

    [SerializeField] private List<Collider2D> CeilingCollisions = new();
    [SerializeField] private List<Collider2D> GroundCollisions = new();
    [SerializeField] private List<Collider2D> WallCollisions = new();

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
        Debug.Log("Contacts: " + collision.contactCount);
        ContactPoint2D[] contactPoints = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contactPoints);
        foreach (var contactPoint in contactPoints)
        {
            Debug.Log(contactPoint.normal);
            if (contactPoint.normal.y > 0.5f && !GroundCollisions.Contains(collision.collider)) // ground touch
            {
                GroundCollisions.Add(collision.collider);
            }
            else if (contactPoint.normal.y < -0.5f && !CeilingCollisions.Contains(collision.collider)) // ceiling touch
            {
                CeilingCollisions.Add(collision.collider);
            }
            else if (!WallCollisions.Contains(collision.collider)) // assume wall touch
            {
                WallCollisions.Add(collision.collider);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        GroundCollisions.Remove(collision.collider);
        CeilingCollisions.Remove(collision.collider);
        WallCollisions.Remove(collision.collider);
    }

    public void PhysicsMove(Rigidbody2D rb)
    {
        rb.linearVelocity = CurrentVelocity;
    }

}
