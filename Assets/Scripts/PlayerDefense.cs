using UnityEngine;

public class PlayerDefense : MonoBehaviour
{
    [Header("Wall Prefabs")]
    [SerializeField] private GameObject EarthWall;
    [SerializeField] private GameObject WaterWall;
    [SerializeField] private GameObject FireWall;

    [SerializeField] private float spawnDistanceFromPlayer = 3f;
    [SerializeField] private float spawnCoolDown = 3f;

    private float lastSpawnTime = -Mathf.Infinity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SpawnWall(GameObject wallPrefab)
    {
        if (Time.time - lastSpawnTime < spawnCoolDown)
        {
            Debug.Log("Wall on cooldown!");
            return;
        }

        lastSpawnTime = Time.time;

        Vector3 spawnPos = transform.position + transform.forward * spawnDistanceFromPlayer;
        GameObject wall = Instantiate(wallPrefab, spawnPos, Quaternion.identity);

        // Optional: place it buried below ground
        Renderer rend = wall.GetComponentInChildren<Renderer>();
        float height = rend != null ? rend.bounds.size.y : 1f;
        wall.transform.position = new Vector3(spawnPos.x, -height / 2f, spawnPos.z);

        Debug.Log($"Spawned {wallPrefab.name} at {wall.transform.position}");
    }


    // accessible methods to spawn each wall type
    public void SpawnEarthWall()
    {
        SpawnWall(EarthWall);
    }

    public void SpawnWaterWall()
    {
        SpawnWall(WaterWall);
    }

    public void SpawnFireWall()
    {
        SpawnWall(FireWall);
    }

}
