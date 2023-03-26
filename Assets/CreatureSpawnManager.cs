using UnityEngine;

public class CreatureSpawnManager : MonoBehaviour
{
    public GameObject hunterCreaturePrefab;
    public int grazingCreatureThreshold = 30;
    private bool hunterCreatureSpawned = false;

    void Update()
    {
        if (!hunterCreatureSpawned && GameObject.FindGameObjectsWithTag("GrazingCreature").Length >= grazingCreatureThreshold)
        {
            SpawnHunterCreature();

            hunterCreatureSpawned = true;
        }
    }

    void SpawnHunterCreature()
    {
        Instantiate(hunterCreaturePrefab, Vector2.zero, Quaternion.identity);
    }
}
