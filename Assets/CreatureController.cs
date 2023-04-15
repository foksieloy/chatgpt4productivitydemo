using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public float growthRate = 0.05f;
    public float minSize = 0.5f;
    public float maxSize = 2f;
    public float minSpawnInterval = 600f; // 10 minutes
    public float maxSpawnInterval = 20f;

    public float foodGrowthFactor = 0.1f;
    private float foodRating = 10f;
    public float foodLossRate = 0.005f;
    public float initialFoodRating = 10f;
    public float maxFoodRating = 100f;

    public Color lightColor = Color.Lerp(Color.green, Color.white, 0.5f);
    public Color darkColor = Color.Lerp(Color.green, Color.black, 0.5f);

    protected float spawnTimer;
    protected Renderer objectRenderer;

    protected BackgroundController backgroundController;

    public float FoodRating { get => foodRating; set => foodRating = value; }
    public Color CurrentColor
    {
        get
        {
            float colorLerpValue = Mathf.InverseLerp(minSize, maxSize, CurrentSize);
            Color newColor = Color.Lerp(lightColor, darkColor, colorLerpValue);
            return newColor;
        }

    }

    public float CurrentSize
    {
        get
        {
            float currentSize = Mathf.Lerp(minSize, maxSize, FoodRating / maxFoodRating);
            return currentSize;
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        backgroundController = FindObjectOfType<BackgroundController>();
    }

    protected void Update()
    {
        CheckDeath();
        Grow();
        Split();
        Move();
    }

    protected virtual void Move()
    {
        // Implement common movement logic
    }

    protected virtual void Grow()
    {
        
        transform.localScale = new(CurrentSize, CurrentSize, CurrentSize);

        GetComponent<Renderer>().material.color = CurrentColor;
    }

    protected virtual void Split()
    {
        // Implement common splitting logic
    }

    protected void CheckDeath()
    {
        if (FoodRating <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
