using System;
using UnityEngine;

public class FoodCreatureController : PlantCreatureController
{
    new protected void Start()
    {
        base.Start();
        objectRenderer = GetComponent<Renderer>();
        spawnTimer = GetRandomSpawnInterval();
        backgroundController = FindObjectOfType<BackgroundController>();

        maxSize = 5.0f;
        maxFoodRating = 200.0f;
        growthRate = 0.5f;
    }

    new protected void Update()
    {
        base.Update();
        GrowFoodCreature();
        SpawnSmallFoodCreature();
    }

    void GrowFoodCreature()
    {
        if (transform.localScale.x < maxSize)
        {
            Color colorAtPosition = backgroundController.GetColorAtWorldPosition(transform.position);
            float growthMultiplier = Mathf.InverseLerp(0, 1, colorAtPosition.b); // Assuming more cyan means higher blue value

            float growthAmount = growthRate * growthMultiplier * Time.deltaTime;
            FoodRating += growthAmount;
        }
    }


    void SpawnSmallFoodCreature()
    {
        float spawnProbability = Mathf.Lerp(1 / minSpawnInterval, 1 / maxSpawnInterval, transform.localScale.x / maxSize);

        if (UnityEngine.Random.value < spawnProbability * Time.deltaTime)
        {
            float spawnRadius = 1f;
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)transform.position + randomDirection * spawnRadius;

            GameObject newFoodCreature = Instantiate(gameObject, spawnPosition, Quaternion.identity);
            newFoodCreature.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }


    float GetRandomSpawnInterval()
    {
        float sizePercentage = transform.localScale.x / maxSize;
        float spawnInterval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, sizePercentage);
        return UnityEngine.Random.Range(spawnInterval * 0.8f, spawnInterval * 1.2f);
    }
}
