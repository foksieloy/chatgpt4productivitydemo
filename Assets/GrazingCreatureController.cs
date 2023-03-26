using System.Collections.Generic;
using UnityEngine;

public class GrazingCreatureController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float foodGrowthFactor = 0.1f;
    public float minDistanceToEat = 0.1f;

    private GameObject targetFoodCreature;
    public float foodRating = 10f;

    public float foodLossRate = 0.005f;

    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    private float idleTime;
    private Vector3 lastPosition;
    private float timeSinceLastMovement;

    public float hunterAwarenessRadius = 5f;

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        lastPosition = transform.position;
        Color color = Color.Lerp(Color.red, Color.black, 0.5f);
        objectRenderer.material.color = color;
    }

    void Update()
    {
        if (targetFoodCreature == null)
        {
            FindClosestFoodCreature();
        }
        else
        {
            MoveTowardsClosestFoodCreature();
            CheckForEating();
        }

        LoseFoodOverTime();
        CheckSplit();
        CheckDeath();
        CheckIdleTime();
        DetectNearbyHunter();
    }

    void DetectNearbyHunter()
    {
        GameObject[] hunterCreatures = GameObject.FindGameObjectsWithTag("HunterCreature");
        float closestDistanceSqr = Mathf.Infinity;
        GameObject closestHunter = null;

        foreach (GameObject hunterCreature in hunterCreatures)
        {
            Vector3 directionToHunter = hunterCreature.transform.position - transform.position;
            float distanceSqrToHunter = directionToHunter.sqrMagnitude;

            if (distanceSqrToHunter < hunterAwarenessRadius * hunterAwarenessRadius && distanceSqrToHunter < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToHunter;
                closestHunter = hunterCreature;
            }
        }

        if (closestHunter != null)
        {
            HunterCreatureController hunterController = closestHunter.GetComponent<HunterCreatureController>();
            GameObject targetedGrazingCreature = hunterController.target;

            if (foodRating < 20)
            {
                // Move away from the nearby hunter
                transform.position += (transform.position - closestHunter.transform.position).normalized * moveSpeed * Time.deltaTime;
            }
            else if (targetedGrazingCreature != null)
            {
                Vector3 directionToTargetedGrazingCreature = targetedGrazingCreature.transform.position - transform.position;

                if (directionToTargetedGrazingCreature.sqrMagnitude < hunterAwarenessRadius * hunterAwarenessRadius)
                {
                    // Move to block the hunter
                    Vector3 blockingPosition = targetedGrazingCreature.transform.position - closestHunter.transform.position;
                    transform.position = Vector3.MoveTowards(transform.position, blockingPosition, moveSpeed * Time.deltaTime);
                }
            }
        }
    }

    void LoseFoodOverTime()
    {
        foodRating -= foodLossRate * Time.deltaTime;
    }

    void CheckIdleTime()
    {
        if (Vector3.Distance(lastPosition, transform.position) < 0.01f)
        {
            timeSinceLastMovement += Time.deltaTime;
            if (timeSinceLastMovement >= idleTime)
            {
                timeSinceLastMovement = 0;
                idleTime = Random.Range(minIdleTime, maxIdleTime);
                FindNewTarget();
            }
        }
        else
        {
            timeSinceLastMovement = 0;
        }

        lastPosition = transform.position;
    }

    void FindNewTarget()
    {
        GameObject[] foodCreatures = GameObject.FindGameObjectsWithTag("FoodCreature");
        if (foodCreatures.Length > 0)
        {
            int randomIndex = Random.Range(0, foodCreatures.Length);
            targetFoodCreature = foodCreatures[randomIndex];
        }
        else
        {
            targetFoodCreature = null;
        }
    }

    void CheckSplit()
    {
        if (foodRating >= 30f)
        {
            // Calculate a random spawn position within a small radius
            float spawnRadius = 4f;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)transform.position + randomDirection * spawnRadius;

            // Instantiate the new GrazingCreature
            GameObject newGrazingCreature = Instantiate(gameObject, spawnPosition, Quaternion.identity);
            newGrazingCreature.GetComponent<GrazingCreatureController>().foodRating = 10f;

            // Reduce the food rating of the current creature
            foodRating = 10f;
        }
    }


    void CheckDeath()
    {
        if (foodRating <= 0f)
        {
            Destroy(gameObject);
        }
    }


    void FindClosestFoodCreature()
    {
        GameObject[] foodCreatures = GameObject.FindGameObjectsWithTag("FoodCreature");
        float closestDistance = Mathf.Infinity;

        foreach (GameObject foodCreature in foodCreatures)
        {
            float distance = Vector2.Distance(transform.position, foodCreature.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetFoodCreature = foodCreature;
            }
        }
    }

    void MoveTowardsClosestFoodCreature()
    {
        GameObject closestFoodCreature = targetFoodCreature;

        if (closestFoodCreature != null)
        {
            Vector2 direction = (Vector2)(closestFoodCreature.transform.position - transform.position).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * moveSpeed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }


    void CheckForEating()
    {
        float distance = Vector2.Distance(transform.position, targetFoodCreature.transform.position);
        if (distance <= minDistanceToEat)
        {
            float foodSize = targetFoodCreature.transform.localScale.x;
            foodRating += foodSize;

            Destroy(targetFoodCreature);
            targetFoodCreature = null;
        }
    }
}
