using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [Header("Projectile Settings")]
    [SerializeField] private float spawnOffset = 1f;
    [SerializeField] private float projectileSpeed = 25f;

    [Header("Attack Objects")]
    [SerializeField] private GameObject fireBall;
    [SerializeField] private GameObject earthBall;
    [SerializeField] private GameObject waterBall;

    [Header("Cooldowns (seconds)")]
    [SerializeField] private float fireCooldown = 3.0f;
    [SerializeField] private float earthCooldown = 3.0f;
    [SerializeField] private float waterCooldown = 3.0f;

    private float nextFireTime = 0f;
    private float nextEarthTime = 0f;
    private float nextWaterTime = 0f;


    public void FireAttack()
    {
        TryFire(fireBall, ref nextFireTime, fireCooldown);
    }
    public void EarthAttack()
    {
        TryFire(earthBall, ref nextEarthTime, earthCooldown);
    }
    public void WaterAttack()
    {
        TryFire(waterBall, ref nextWaterTime, waterCooldown);
    }

    private bool TryFire(GameObject prefab, ref float nextAllowedTime, float cooldown)
    {
        if (Time.time < nextAllowedTime) return false;  // No Shot

        nextAllowedTime = Time.time + cooldown;

        // Spawn a little in front of the player and facing the same direction
        Vector3 spawnPos = transform.position + transform.forward * spawnOffset;
        Quaternion spawnRot = transform.rotation;

        GameObject projectile = Instantiate(prefab, spawnPos, spawnRot);

        if (projectile.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.linearDamping = 0;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.linearVelocity = transform.forward * projectileSpeed;
        }

        return true;    // Successful Shot
    }

}
