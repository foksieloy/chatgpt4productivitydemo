using System.Collections.Generic;
using UnityEngine;

public class GrazingCreatureController : AnimalCreatureController
{

    public float minDistanceToEat = 0.1f;

    private GameObject targetFoodCreature;

    public float minIdleTime = 2f;
    public float maxIdleTime = 5f;
    private float idleTime;
    private Vector3 lastPosition;
    private float timeSinceLastMovement;

    public float hunterAwarenessRadius = 5f;

    new protected void Start()
    {
        base.Start();
        objectRenderer = GetComponent<Renderer>();
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        lastPosition = transform.position;
        lightColor = Color.Lerp(Color.red, Color.white, 0.5f);
        darkColor = Color.Lerp(Color.red, Color.black, 0.5f);
        eatRate = 3f;
        eatDistance = 1f;
    }

    new protected void Update()
    {
        base.Update();
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

            if (FoodRating < 20)
            {
                // Move away from the nearby hunter
                Transform localTransform = transform;
                localTransform.position += moveSpeed * Time.deltaTime * (localTransform.position - closestHunter.transform.position).normalized;
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
        FoodRating -= foodLossRate * Time.deltaTime;
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
        if (FoodRating >= 30f)
        {
            // Calculate a random spawn position within a small radius
            float spawnRadius = 4f;
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)transform.position + randomDirection * spawnRadius;

            // Instantiate the new GrazingCreature
            GameObject newGrazingCreature = Instantiate(gameObject, spawnPosition, Quaternion.identity);
            newGrazingCreature.GetComponent<GrazingCreatureController>().FoodRating = 10f;

            // Reduce the food rating of the current creature
            FoodRating = 10f;
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
        FoodCreatureController targetFoodCreatureController = targetFoodCreature.GetComponent<FoodCreatureController>();
        if (distance <= minDistanceToEat)
        {
            float foodEaten = Mathf.Min(targetFoodCreatureController.FoodRating, eatRate * Time.deltaTime);

            targetFoodCreatureController.FoodRating -= foodEaten;
            FoodRating += foodEaten;
        }
    }
}
