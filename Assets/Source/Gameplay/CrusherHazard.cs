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

    [SerializeField] SFX SoundEffects;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _startPosition = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _timeContext.OnPause += Pause;
    }

    void OnDestroy()
    {
        _timeContext.OnPause -= Pause;
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

    void Pause(bool paused)
    {
        _rb.simulated = !paused;
    }

    private void StopAndTurn(bool rising)
    {
        _rb.linearVelocity = Vector2.zero;
        _remainingStopTime = StopTime;
        _rising = rising;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer & GroundMask.value) == collision.gameObject.layer)
        {
            BGMHandler.Instance.PlaySFX(SoundEffects.heavy);

            StopAndTurn(true);
        }
    }

    protected override void OnContactWithKillable(IKillable killable)
    {
        if (!_rising)
        {
            if (killable.CanBeKilledBy(this))
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
