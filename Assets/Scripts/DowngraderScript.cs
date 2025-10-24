using UnityEngine;

public class DowngraderScript : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private static readonly string[] EndingTags = { "Fire", "Water", "Earth" };

    private void Awake()
    {
        // Prefer an inspector-assigned reference; fall back to a singleton Instance if available.
        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null)
            Debug.LogWarning("DowngraderScript: GameManager not assigned in inspector and GameManager.Instance is null. Assign via inspector or ensure GameManager sets Instance in Awake.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other == null || gameManager == null) return;

        foreach (var tag in EndingTags)
        {
            if (other.CompareTag(tag))
            {
                // play the attached audio
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                //gameManager.EndGame();
                gameManager.RegisterPlayerHit(); // Example action; replace with desired behavior
                return;
            }
        }
    }
}
