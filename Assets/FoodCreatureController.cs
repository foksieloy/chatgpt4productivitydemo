using System;
using UnityEngine;

public class FoodCreatureController : MonoBehaviour
{
    public float growthRate = 0.05f;
    public float maxSize = 2f;
    public float minSpawnInterval = 600f; // 10 minutes
    public float maxSpawnInterval = 20f;

    private float spawnTimer;
    private Renderer objectRenderer;

    private BackgroundController backgroundController;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        spawnTimer = GetRandomSpawnInterval();
        backgroundController = FindObjectOfType<BackgroundController>();

    }

    void Update()
    {
        GrowFoodCreature();
        SpawnSmallFoodCreature();
    }

    void UpdateColor()
    {
        float sizePercentage = transform.localScale.x / maxSize;
        Color color = Color.Lerp(Color.Lerp(Color.green, Color.white, 0.5f), Color.Lerp(Color.green, Color.black, 0.5f), sizePercentage);
        objectRenderer.material.color = color;
    }

    void GrowFoodCreature()
    {
        if (transform.localScale.x < maxSize)
        {
            Color colorAtPosition = backgroundController.GetColorAtWorldPosition(transform.position);
            float growthMultiplier = Mathf.InverseLerp(0, 1, colorAtPosition.b); // Assuming more cyan means higher blue value
            if (UnityEngine.Random.Range(0, 1) > 0.99)
            {
                Debug.Log("The color is: " + growthMultiplier.ToString());
            }


            float growthAmount = growthRate * growthMultiplier * Time.deltaTime;
            Vector3 newScale = transform.localScale + new Vector3(growthAmount, growthAmount, growthAmount);
            newScale = Vector3.Min(newScale, new Vector3(maxSize, maxSize, maxSize));
            transform.localScale = newScale;
            UpdateColor();
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
