using System.Collections.Generic;
using UnityEngine;

public class HunterCreatureController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float foodLossRate = 0.1f;
    public float foodRating = 15f;
    private GameObject closestGrazingCreature;
    private Renderer objectRenderer;
    public GameObject target;
    public float eatDistance = 1f;
    public GameObject hunterCreatureBodyPrefab;

    private List<GameObject> bodyParts = new List<GameObject>();
    private float minBodyPartDistance = 0.5f;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        Color color = Color.Lerp(Color.red, Color.white, 0.6f);
        objectRenderer.material.color = color;
    }

    private Color GetColor()
    {
        float colorLerpValue = Mathf.InverseLerp(0, 100, foodRating);
        return Color.Lerp(Color.red, Color.white, colorLerpValue);
    }

    void Update()
    {
        MoveTowardsClosestGrazingCreature();
        LoseFoodOverTime();
        CheckDeath();
        MoveBody();
        CheckSplit();
    }

    private void MoveBody()
    {
        int targetBodyParts = Mathf.Clamp(Mathf.RoundToInt(foodRating / 10), 1, 10);
        Color bodyPartColor = GetColor();

        while (bodyParts.Count < targetBodyParts)
        {
            AddBodyPart();
        }
        while (bodyParts.Count > targetBodyParts)
        {
            GameObject lastBodyPart = bodyParts[bodyParts.Count - 1];
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
        Color color = Color.Lerp(Color.red, Color.white, 0.6f);
        newBodyPart.GetComponent<Renderer>().material.color = color;
        bodyParts.Add(newBodyPart);
    }


    private void CheckSplit()
    {
        if (foodRating >= 100)
        {
            foodRating = 2;
            GameObject newHunterCreature = Instantiate(gameObject, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0), Quaternion.identity);
            HunterCreatureController newHunterCreatureController = newHunterCreature.GetComponent<HunterCreatureController>();
            newHunterCreatureController.foodRating = 2;

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
            if (grazingCreatureController.foodRating > 20)
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
        foodRating -= foodLossRate * Time.deltaTime;
    }

    void CheckDeath()
    {
        if (foodRating <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void TryToEatGrazingCreature(GrazingCreatureController grazingCreature)
    {
        if (grazingCreature == null) return;

        float distanceToGrazingCreature = Vector3.Distance(transform.position, grazingCreature.transform.position);
        if (distanceToGrazingCreature <= eatDistance)
        {
            if (grazingCreature.foodRating <= 20)
            {
                foodRating += grazingCreature.foodRating;
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
