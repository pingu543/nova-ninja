using UnityEngine;
using UnityEngine.Events;
using System.Collections;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

public class DummyElement : MonoBehaviour
{
    // Public variable with a drop down to select element type (fire, water, earth).
    public enum ElementType { None, Fire, Water, Earth }
    public ElementType elementType;
    // Public variable for element materials
    public Material noneMaterial;
    public Material fireMaterial;
    public Material waterMaterial;
    public Material earthMaterial;

    // public variable to hold an explosion prefab for defeat effect
    public GameObject explosionPrefab;
    // explosion size
    public float explosionSize = 1.0f;
    
    // Event invoked when the defeat animation finishes (or immediately if no animator)
    public UnityEvent onDefeatAnimationComplete;

    // Internal coroutine reference for animation watching
    Coroutine _watchDefeatCoroutine;

    void OnEnable()
    {
        if (onDefeatAnimationComplete == null)
            onDefeatAnimationComplete = new UnityEvent();
        onDefeatAnimationComplete.RemoveListener(SpawnExplosion);
        onDefeatAnimationComplete.AddListener(SpawnExplosion);
    }

    void OnDisable()
    {
        if (onDefeatAnimationComplete != null)
            onDefeatAnimationComplete.RemoveListener(SpawnExplosion);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Assign material based on element type
        if (!TryGetComponent<Renderer>(out var rend))
        {
            // Try to find a renderer on children as a fallback
            rend = GetComponentInChildren<Renderer>();
            if (rend == null)
            {
                Debug.LogError($"[{nameof(DummyElement)}] Renderer is missing on '{gameObject.name}'. Please add a Renderer (MeshRenderer/SpriteRenderer) to this GameObject or one of its children.");
                // Disable this component to avoid null-reference errors during runtime
                enabled = false;
                return;
            }
        }

        Material selected = null;
        switch (elementType)
        {
            case ElementType.Fire:
                selected = fireMaterial;
                break;
            case ElementType.Water:
                selected = waterMaterial;
                break;
            case ElementType.Earth:
                selected = earthMaterial;
                break;
            default:
                selected = noneMaterial;
                break;
        }

        if (selected == null)
        {
            Debug.LogWarning($"[{nameof(DummyElement)}] No material assigned for element type '{elementType}' on '{gameObject.name}'.");
        }
        else
        {
            rend.material = selected;
        }

        // Set the GameObject's tag to match the element (or Untagged for None)
        TrySetTagForElement(elementType);

        // Pause the animation at the start        
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.enabled = false;
        }
    }

    // react to collisions with objects with different element types.
    // compare the tag against object's own element type.
    // Fire > Earth, Earth > Water, Water > Fire
    // On collision with a stronger element, trigger defeat behavior.
    // Otherwise, trigger a counterattack.
    void OnCollisionEnter(Collision collision)
    {
        // find which element tag the other object has
        ElementType otherElement = ElementType.None;
        if (collision.gameObject.CompareTag("Fire"))
        {
            otherElement = ElementType.Fire;
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            otherElement = ElementType.Water;
        }
        else if (collision.gameObject.CompareTag("Earth"))
        {
            otherElement = ElementType.Earth;
        }

        // Compare the element types and trigger appropriate behavior
        if (otherElement != ElementType.None)
        {
            if (IsStrongerElement(otherElement))
            {
                OnDefeat();
            }
            else
            {
                CounterAttack();
            }
        }
    }

    bool IsStrongerElement(ElementType other)
    {
        // Determine if the other element is stronger than this object's element
        switch (elementType)
        {
            case ElementType.Fire:
                return other == ElementType.Water; // Water beats Fire
            case ElementType.Water:
                return other == ElementType.Earth; // Earth beats Water
            case ElementType.Earth:
                return other == ElementType.Fire; // Fire beats Earth
            default:
                return false; // None has no strengths or weaknesses
        }
    }


    void OnDefeat()
    {

        // play the second sound attached to this dummy (not through SoundManager)
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2 && audioSources[1] != null)
        {
            audioSources[1].Play();
        }
        // Resume the animation when defeated
        if (TryGetComponent<Animator>(out var animator2))
        {
            animator2.enabled = true;
            // start watching for the end of the current/next animation
            // stop previous watcher if any
            if (_watchDefeatCoroutine != null)
                StopCoroutine(_watchDefeatCoroutine);
            _watchDefeatCoroutine = StartCoroutine(WatchAnimatorForEnd(animator2));
        }
        // Turn off the collider to prevent further interactions
        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }
        // Turn off rigidbody physics
        if (TryGetComponent<Rigidbody>(out var rb2))
        {
            rb2.isKinematic = true;
        }
        // Check if the object also has a DummyFaceCamera component and disable it
        var faceCam = GetComponent<DummyFaceCamera>();
        if (faceCam != null)
        {
            faceCam.enabled = false;
        }
        // If there's no animator, invoke the completion immediately so explosion still spawns
        if (!TryGetComponent<Animator>(out var anim))
        {
            onDefeatAnimationComplete?.Invoke();
            return;
        }

    }

    void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            // spawn the explosion on the position of this object plus a unit up
            var explosion = Instantiate(explosionPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            // display in debug of the position of the explosion
            Debug.Log($"[{nameof(DummyElement)}] Explosion spawned at {explosion.transform.position} for '{gameObject.name}'.");

            // apply explosion size
            explosion.transform.localScale = Vector3.one * explosionSize;
        }
        else
        {
            Debug.LogWarning($"[{nameof(DummyElement)}] Explosion prefab is not assigned on '{gameObject.name}'.");
        }
    }

    IEnumerator WatchAnimatorForEnd(Animator animator)
    {
        if (animator == null)
            yield break;

        var layer = 0;

        // Wait until animator has at least one valid state (avoid 0-length clips)
        while (animator.enabled && !animator.GetCurrentAnimatorStateInfo(layer).loop && animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 0f)
        {
            yield return null;
        }

        // Now wait for the state's normalizedTime to reach >= 1.0 (animation finished)
        while (animator.enabled)
        {
            var state = animator.GetCurrentAnimatorStateInfo(layer);
            // If the clip is looping, we can't detect an 'end' reliably here; break out and invoke immediately
            if (state.loop)
                break;

            if (state.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        // Animation considered finished; invoke event and clear coroutine ref
        onDefeatAnimationComplete?.Invoke();
        _watchDefeatCoroutine = null;
    }

    void CounterAttack()
    {
        // PlayerAttack object is attached to the dummy.
        // Call the FireAttack, EarthAttack, or WaterAttack method based on this dummy's element type.
        var playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack != null)
        {
            switch (elementType)
            {
                case ElementType.Fire:
                    playerAttack.FireAttack();
                    break;
                case ElementType.Water:
                    playerAttack.WaterAttack();
                    break;
                case ElementType.Earth:
                    playerAttack.EarthAttack();
                    break;
                default:
                    Debug.LogWarning($"[{nameof(DummyElement)}] No counterattack defined for element type '{elementType}' on '{gameObject.name}'.");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[{nameof(DummyElement)}] PlayerAttack component not found on '{gameObject.name}' for counterattack.");
        }
        // play the first sound attached to this dummy (not through SoundManager)
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 1 && audioSources[0] != null)
        {
            audioSources[0].Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Editor: apply material changes immediately when inspector values change
    void OnValidate()
    {
        // Don't perform editor-only logic if the component is disabled
        if (!isActiveAndEnabled)
            return;

        // Try to find renderer (same logic as Start)
        if (!TryGetComponent<Renderer>(out var rend))
        {
            rend = GetComponentInChildren<Renderer>();
            if (rend == null)
            {
                return; // nothing to update in editor
            }
        }

        Material selected = null;
        switch (elementType)
        {
            case ElementType.Fire:
                selected = fireMaterial;
                break;
            case ElementType.Water:
                selected = waterMaterial;
                break;
            case ElementType.Earth:
                selected = earthMaterial;
                break;
            default:
                selected = noneMaterial;
                break;
        }

        if (selected != null)
        {
            // Use sharedMaterial in editor to avoid creating instances unintentionally
            rend.sharedMaterial = selected;
        }

#if UNITY_EDITOR
        // mark dirty so changes persist and repaint scene view
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneView.RepaintAll();
#endif

        // Ensure the tag matches the selected element in the editor as well
        TrySetTagForElement(elementType);
    }

    /// <summary>
    /// Try to set the GameObject tag based on the element type.
    /// - "Fire", "Water", "Earth" tags are used for the respective elements.
    /// - "Untagged" is used for None.
    /// If the tag does not exist in the project's Tag Manager, a warning is logged with guidance.
    /// </summary>
    void TrySetTagForElement(ElementType elem)
    {
        string tagName = elem == ElementType.None ? "Untagged" : elem.ToString();

#if UNITY_EDITOR
        // In editor we can check available tags
        var tags = InternalEditorUtility.tags; // UnityEditorInternal
        if (tagName == "Untagged" || System.Array.IndexOf(tags, tagName) >= 0)
        {
            try
            {
                gameObject.tag = tagName;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[{nameof(DummyElement)}] Failed to set tag '{tagName}' on '{gameObject.name}': {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"[{nameof(DummyElement)}] Tag '{tagName}' is not defined in the project's Tag Manager. Add it via the Tag Manager (Add Tag) before running. GameObject '{gameObject.name}' remains with tag '{gameObject.tag}'.");
        }
#else
        // At runtime, setting an undefined tag will throw; handle that gracefully.
        try
        {
            gameObject.tag = tagName;
        }
        catch (UnityEngine.UnityException)
        {
            Debug.LogWarning($"[{nameof(DummyElement)}] Tag '{tagName}' is not defined in this build for GameObject '{gameObject.name}'. Define the tag in the Editor's Tag Manager and rebuild. Current tag remains '{gameObject.tag}'.");
        }
#endif
    }
}
