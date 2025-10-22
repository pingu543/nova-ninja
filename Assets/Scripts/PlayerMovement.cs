using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Core Movement")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float turnSpeedDeg = 180f;
    [SerializeField] private float turnSmoothTime = 0.08f; // smoothing time in seconds

    [Header("Dash")]
    [SerializeField] private float dashMultiplier = 2.0f;
    [SerializeField] private float dashCooldown = 3.0f;

    [Header("Gesture sustain")]
    [Tooltip("Your camera reassesses every ~0.5s. Give a small buffer.")]
    [SerializeField] float reassessPeriod = 0.5f;
    [SerializeField] float sustainBuffer = 0.1f; // total sustain = 0.6s

    Rigidbody rb;

    // last pulse state
    float lastTurnPulseTime = -999f;
    int lastTurnDir = 0; // -1 left, +1 right, 0 none

    float turnInputSmoothed = 0f;
    float turnInputVelRef = 0f;

    // (optional) forward/back sustain too
    float lastMovePulseTime = -999f;
    int lastMoveDir = 0; // -1 back, +1 fwd

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // ====== Gesture pulse handlers (call whenever the system reports a detection) ======
    public void TurnLeftPulse() { lastTurnDir = -1; lastTurnPulseTime = Time.time; }
    public void TurnRightPulse() { lastTurnDir = +1; lastTurnPulseTime = Time.time; }
    public void ForwardPulse() { lastMoveDir = +1; lastMovePulseTime = Time.time; }
    public void BackwardPulse() { lastMoveDir = -1; lastMovePulseTime = Time.time; }
    // ================================================================================

    void FixedUpdate()
    {
        float now = Time.time;
        float sustain = reassessPeriod + sustainBuffer;

        // TURN: active while within sustain window
        bool turnActive = (now - lastTurnPulseTime) <= sustain;
        float targetTurnInput = turnActive ? lastTurnDir : 0f;

        turnInputSmoothed = Mathf.SmoothDamp(
            turnInputSmoothed,
            targetTurnInput,
            ref turnInputVelRef,
            turnSmoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        float yawDeg = turnInputSmoothed * turnSpeedDeg * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yawDeg, 0f));

        // MOVE: same sustain idea (optional)
        bool moveActive = (now - lastMovePulseTime) <= sustain;
        if (moveActive && lastMoveDir != 0)
        {
            rb.AddForce(transform.forward * (lastMoveDir * movementSpeed), ForceMode.Acceleration);
        }
    }
}
