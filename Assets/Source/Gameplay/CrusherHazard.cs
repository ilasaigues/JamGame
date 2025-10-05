using AstralCore;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class CrusherHazard : BaseHazard
{
    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb;

    private Vector3 _startPosition;
    [SerializeField] private LayerMask GroundMask;

    [SerializeField] private float VerticalVelocity = 5;

    [SerializeField] private float StopTime = 1;

    private bool _rising = false;

    private float _remainingStopTime;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _startPosition = transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (_remainingStopTime > 0)
        {
            _remainingStopTime -= FixedDeltaTime;
        }
        else
        {
            _rb.linearVelocity = Vector3.up * (_rising ? VerticalVelocity : -VerticalVelocity);
            if (_rising && transform.position.y >= _startPosition.y)
            {
                StopAndTurn(false);
            }
        }
    }

    private void StopAndTurn(bool rising)
    {
        _rb.linearVelocity = Vector2.zero;
        _remainingStopTime = StopTime;
        _rising = rising;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Colliding with " + collision.gameObject.name);
        if ((collision.gameObject.layer & GroundMask.value) == collision.gameObject.layer)
        {
            StopAndTurn(true);
        }
    }

    protected override void OnContactWithKillable(IKillable killable)
    {
        if (!_rising)
        {
            if (killable.CanBeKilledBy(this).LogInPlace())
            {
                StartCoroutine(killable.Kill(this));
            }
            else
            {
                StopAndTurn(true);
            }
        }
    }
}
