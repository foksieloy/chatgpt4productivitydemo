using System.Collections.Generic;
using UnityEngine;

public class HunterCreatureController : AnimalCreatureController
{
 
    private GameObject closestGrazingCreature;
    public GameObject target;
    
    public GameObject hunterCreatureBodyPrefab;

    private readonly List<GameObject> bodyParts = new();
    private readonly float minBodyPartDistance = 0.5f;

    new protected void Start()
    {
        base.Start();
        objectRenderer = GetComponent<Renderer>();
        lightColor = new Color(0.2f, 0.2f, 0.2f);
        darkColor = new Color(0.7f, 0.7f, 0.7f);
        moveSpeed = 5;
    }

    new protected void Update()
    {
        base.Update();
        MoveTowardsClosestGrazingCreature();
        LoseFoodOverTime();
        CheckDeath();
        MoveBody();
        CheckSplit();
    }

    private void MoveBody()
    {
        int targetBodyParts = Mathf.Clamp(Mathf.RoundToInt(FoodRating / 10), 1, 10);
        _ = CurrentColor;

        while (bodyParts.Count < targetBodyParts)
        {
            AddBodyPart();
        }
        while (bodyParts.Count > targetBodyParts)
        {
            GameObject lastBodyPart = bodyParts[^1];
            bodyParts.Remove(lastBodyPart);
            Destroy(lastBodyPart);
        }

        Vector3 previousPosition = transform.position;

        for (int i = 0; i < bodyParts.Count; i++)
        {
            GameObject bodyPart = bodyParts[i];
            float distance = Vector3.Distance(previousPosition, bodyPart.transform.position);

            if (distance > minBodyPartDistance)
            {
                Vector3 newPosition = Vector3.MoveTowards(bodyPart.transform.position, previousPosition, distance - minBodyPartDistance);
                bodyPart.transform.position = newPosition;
            }

            previousPosition = bodyPart.transform.position;
        }

    }

    private void AddBodyPart()
    {
        GameObject newBodyPart = Instantiate(hunterCreatureBodyPrefab, transform.position, Quaternion.identity);
        Color color = CurrentColor;
        newBodyPart.GetComponent<Renderer>().material.color = color;
        bodyParts.Add(newBodyPart);
    }


    private void CheckSplit()
    {
        if (FoodRating >= 100)
        {
            FoodRating = 2;
            GameObject newHunterCreature = Instantiate(gameObject, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
            HunterCreatureController newHunterCreatureController = newHunterCreature.GetComponent<HunterCreatureController>();
            newHunterCreatureController.FoodRating = 2;

            foreach (GameObject bodyPart in bodyParts)
            {
                Destroy(bodyPart);
            }
            bodyParts.Clear();
        }
    }

    void MoveTowardsClosestGrazingCreature()
    {
        closestGrazingCreature = FindClosestGrazingCreature();
        if (closestGrazingCreature != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestGrazingCreature.transform.position, moveSpeed * Time.deltaTime);
            target = closestGrazingCreature;
        }
    }

    GameObject FindClosestGrazingCreature()
    {
        GameObject[] grazingCreatures = GameObject.FindGameObjectsWithTag("GrazingCreature");
        GameObject closestGrazingCreature = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (GameObject grazingCreature in grazingCreatures)
        {
            GrazingCreatureController grazingCreatureController = grazingCreature.GetComponent<GrazingCreatureController>();
            if (grazingCreatureController.FoodRating > 20)
            {
                continue; // Skip GrazingCreatures with more than 20 food
            }

            Vector3 directionToGrazingCreature = grazingCreature.transform.position - transform.position;
            float distanceSqrToGrazingCreature = directionToGrazingCreature.sqrMagnitude;

            if (distanceSqrToGrazingCreature < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToGrazingCreature;
                closestGrazingCreature = grazingCreature;
            }
        }

        return closestGrazingCreature;
    }


    void LoseFoodOverTime()
    {
        FoodRating -= foodLossRate * Time.deltaTime;
    }

    private void TryToEatGrazingCreature(GrazingCreatureController grazingCreature)
    {
        if (grazingCreature == null) return;

        float distanceToGrazingCreature = Vector3.Distance(transform.position, grazingCreature.transform.position);
        if (distanceToGrazingCreature <= eatDistance)
        {
            if (grazingCreature.FoodRating <= 20)
            {
                FoodRating += grazingCreature.FoodRating;
                Destroy(grazingCreature.gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I colided with food!");
        GrazingCreatureController grazingCreature = other.gameObject.GetComponent<GrazingCreatureController>();
        TryToEatGrazingCreature(grazingCreature);
    }
}
