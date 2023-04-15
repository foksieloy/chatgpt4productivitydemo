using UnityEngine;

public class AnimalCreatureController : CreatureController
{
    // Animal-specific functionality and properties
    public float moveSpeed = 5f;
    public float eatDistance = 1f;
    public float foodRatingGrowthRate = 0.1f;
    public float foodRatingLossRate = 0.005f;
    public float splitFoodRatingThreshold = 50f;
    public float splitDistance = 1f;

    public float eatRate = 1f;

    new protected void Start()
    {
        base.Start(); // Call base class's Start method
        // Implement animal-specific initialization logic
    }

    protected override void Move()
    {
        // Implement animal-specific movement logic
        base.Move(); // Call base class's method if necessary
    }
}
