using UnityEngine;

public class FireAttack : MonoBehaviour
{
    //[SerializeField] private float projectileSpeed = 10.0f;
    [SerializeField] private float projectileLife = 5.0f;


    Renderer renderer;
    //Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.firebrick;
        // rb = GetComponent<Rigidbody>(); //don't need because movement has moved to PlayerAttack
        Destroy(gameObject, projectileLife);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
