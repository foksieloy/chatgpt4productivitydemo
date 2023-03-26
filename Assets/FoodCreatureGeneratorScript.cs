using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCreatureGeneratorScript : MonoBehaviour
{
    public GameObject foodCreaturePrefab;
    public GameObject grazingCreaturePrefab;
    public int numberOfCreatures = 300;
    public Vector2 spawnAreaSize = new Vector2(100f, 100f);
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
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            0
        );

        Instantiate(grazingCreaturePrefab, spawnPosition, Quaternion.identity);
    }


    void SpawnFoodCreature()
    {
        Vector2 spawnPosition = new Vector2(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );
        GameObject spawnedCreature = Instantiate(foodCreaturePrefab, spawnPosition, Quaternion.identity);

        // Randomize scale
        float minScale = 0.5f;
        float maxScale = 1.5f;
        float randomScale = Random.Range(minScale, maxScale);
        spawnedCreature.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        // Adjust color based on scale
        float colorLerpFactor = (randomScale - minScale) / (maxScale - minScale);
        Color lightGreen = new Color(0.4f, 1f, 0.4f);
        Color darkGreen = new Color(0f, 0.5f, 0f);
        Color adjustedColor = Color.Lerp(lightGreen, darkGreen, colorLerpFactor);

        // Apply the new color to the material
        Renderer creatureRenderer = spawnedCreature.GetComponent<Renderer>();
        Material creatureMaterial = creatureRenderer.material;
        creatureMaterial.color = adjustedColor;
    }

    void SpawnFoodCreatures()
    {
        for (int i = 0; i < numberOfCreatures; i++)
        {
            SpawnFoodCreature();
        }
    }
}