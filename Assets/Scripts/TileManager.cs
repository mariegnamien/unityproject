using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public float tileLength = 200;
    public float zSpawn = 0;
    public int numberOfTiles = 6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            if (i == 0)
            {
                SpawnTile(0);
            }
            else
            {
                SpawnTile(Random.Range(0, tilePrefabs.Length));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SpawnTile(int tileIndex)
    {
        Instantiate(tilePrefabs[tileIndex], new Vector3(0, 0, zSpawn), transform.rotation);
        zSpawn += tileLength;

    }
}
