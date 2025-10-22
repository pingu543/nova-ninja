using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Header("Core Movement")]
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float turnSpeed = 1.0f;

    [Header("Dash")]
    [SerializeField] private float dashMultiplier = 2.0f;
    [SerializeField] private float dashCooldown = 3.0f;

    private float nextDashTime = 0f;
    Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Forward()
    {
        rb.AddForce(transform.forward * movementSpeed);
    }

    public void Backward()
    {
        rb.AddForce(-transform.forward * movementSpeed);
    }

    public void TurnLeft()
    {
        transform.Rotate(0, -1 * turnSpeed, 0);
    }

    public void TurnRight()
    {
        transform.Rotate(0, turnSpeed, 0);
    }

    public void Dash()
    {
        TryDash(ref nextDashTime, dashCooldown);
    }

    private bool TryDash(ref float nextAllowedTime, float cooldown)
    {
        if (Time.time < nextAllowedTime) return false;


        rb.AddForce(transform.forward * movementSpeed * dashMultiplier);
        return true;
    }

}
