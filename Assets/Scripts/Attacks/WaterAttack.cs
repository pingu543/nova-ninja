using UnityEngine;

public class WaterAttack : MonoBehaviour
{
    private const string FIRE_TAG = "Fire";
    private const string WATER_TAG = "Water";
    private const string EARTH_TAG = "Earth";
    private const string PLAYER_TAG = "Player";
    private const string TROPHY_TAG = "Trophy";

    //[SerializeField] private float projectileSpeed = 10.0f;
    [SerializeField] public float defaultLifespan = 8f;
    [SerializeField] public float bounceLifespan = 3f;

    private Vector3 preCollisionVelocity;
    private Rigidbody rb;

    Renderer renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //renderer = GetComponent<Renderer>();
        //renderer.material.color = Color.blue;

         rb = GetComponent<Rigidbody>();

        Destroy(gameObject, defaultLifespan);
    }

    private void OnCollisionEnter(Collision collision) // Called when this collider/rigidbody has begun touching another rigidbody/collider
    {
        GameObject otherObject = collision.gameObject;
        string otherTag = otherObject.tag;

        // loss
        if (otherTag == EARTH_TAG)
        {
            Destroy(gameObject);
        }
        // win
        else if (otherTag == FIRE_TAG)
        {
            // if fire wall
            WallMarker wallMarker = otherObject.GetComponent<WallMarker>();
            if (wallMarker != null)
            {
                Destroy(otherObject);
                if (rb != null)
                {
                    rb.linearVelocity = preCollisionVelocity;
                }
            }
            else // if fire projectile
            {
                Destroy(gameObject, bounceLifespan);
            }
                
        }
        // player
        else if (otherTag == PLAYER_TAG)
        {
            Destroy(gameObject);
            // DAMAGE (or lessen rank. But it might exist elsewhere)
        }
        // target?
        //
        // wall?
        //
        // tie (and environment)
        else
        {
            Destroy(gameObject, bounceLifespan);
        }
    }
}
