using UnityEngine;

/// <summary>
/// Make the GameObject face the player's camera.
/// Features:
/// - Optionally assign a specific camera Transform; defaults to Camera.main
/// - Smooth or instant rotation
/// - Lock rotation to Y axis only (useful for 2D-like billboards)
/// - Preserve a custom up vector if needed
/// </summary>
public class DummyFaceCamera : MonoBehaviour
{
    [Tooltip("If assigned, this Transform will be used as the camera target. Otherwise Camera.main is used.")]
    public Transform targetCamera;

    [Tooltip("If true, the object will only rotate around the Y axis to face the camera.")]
    public bool yAxisOnly = true;

    [Tooltip("Smoothing factor for rotation. 0 = instant, larger values = slower smoothing.")]
    [Range(0f, 20f)]
    public float smoothSpeed = 10f;

    [Tooltip("Optional up vector to use when building the look rotation. Default is Vector3.up.")]
    public Vector3 upVector = Vector3.up;

    // Cache reference to the camera transform that will be used each frame
    Transform camTransform;

    // on off toggle for the script
    public bool isEnabled = true;

    void Start()
    {
        if (targetCamera != null)
        {
            camTransform = targetCamera;
        }
        else if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
        else
        {
            // Try to find any camera in the scene as a fallback
            var cam = Object.FindFirstObjectByType<Camera>();
            if (cam != null) camTransform = cam.transform;
        }
    }

    void LateUpdate()
    {
        if (!isEnabled) return;

        // Re-resolve camera if the reference was lost or not set in Start
        if (camTransform == null)
        {
            if (targetCamera != null) camTransform = targetCamera;
            else if (Camera.main != null) camTransform = Camera.main.transform;
            else
            {
                var cam = Object.FindFirstObjectByType<Camera>();
                if (cam != null) camTransform = cam.transform;
                else return; // no camera available
            }
        }

        Vector3 direction = camTransform.position - transform.position;
        if (direction.sqrMagnitude < Mathf.Epsilon) return; // camera at same position

        Quaternion targetRotation;
        if (yAxisOnly)
        {
            // Project direction onto XZ plane so only Y rotation changes
            Vector3 dir = direction;
            dir.y = 0f;
            if (dir.sqrMagnitude < Mathf.Epsilon) return;
            targetRotation = Quaternion.LookRotation(dir, upVector);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(direction, upVector);
        }

        if (smoothSpeed <= 0f)
        {
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));
        }
    }
}
