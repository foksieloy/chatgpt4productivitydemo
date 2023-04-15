using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCreatureGeneratorScript : MonoBehaviour
{
    public GameObject foodCreaturePrefab;
    public GameObject grazingCreaturePrefab;
    public int numberOfCreatures = 300;
    public Vector2 spawnAreaSize = new(100f, 100f);
    public float spawnInterval = 4f;
    private float timeSinceLastSpawn;

    void Start()
    {
        SpawnFoodCreatures();
        for (int i = 0; i < 12; i++)
        {
            SpawnGrazingCreature();
        }
    }

    void Update()
    {
        timeSinceLastSpawn -= Time.deltaTime;

        int foodCreatureCount = GameObject.FindGameObjectsWithTag("FoodCreature").Length;

        if (timeSinceLastSpawn <= 0f && foodCreatureCount < 1000)
        {
            SpawnFoodCreature();
            timeSinceLastSpawn = spawnInterval;
        }
    }

    private void SpawnGrazingCreature()
    {
        Vector3 spawnPosition = new(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            0
        );

        Instantiate(grazingCreaturePrefab, spawnPosition, Quaternion.identity);
    }


    void SpawnFoodCreature()
    {
        Vector2 spawnPosition = new(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );
        GameObject spawnedCreature = Instantiate(foodCreaturePrefab, spawnPosition, Quaternion.identity);
        FoodCreatureController foodCreatureController = spawnedCreature.GetComponent<FoodCreatureController>();
        // Randomize scale
        float initialFoodRating = Random.Range(foodCreatureController.initialFoodRating, foodCreatureController.maxFoodRating);
        foodCreatureController.FoodRating = initialFoodRating;
    }

    void SpawnFoodCreatures()
    {
        for (int i = 0; i < numberOfCreatures; i++)
        {
            SpawnFoodCreature();
        }
    }
}