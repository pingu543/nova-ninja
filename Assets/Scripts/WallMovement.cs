using UnityEngine;

public class WallMovement : MonoBehaviour
{

    [SerializeField] public float WallMovementSpeed = 5f;
    [SerializeField] public float WallLifetime = 3f;

    private float targetHeight;
    private bool isRising = true;
    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Schedule wall destruction
        Destroy(gameObject, WallLifetime);

        // get wall height
        float height = 0f;
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            height = rend.bounds.size.y;
        }
        else
        {
            Collider col = GetComponentInChildren<Collider>();
            if (col != null)
            {
                height = col.bounds.size.y;
            }
        }

        targetPosition = transform.position + new Vector3(0f, height, 0f);

    }

    // Update is called once per frame
    void Update()
    {
        if (!isRising) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, WallMovementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isRising = false;
        }
    }
}
