public class PlantCreatureController : CreatureController
{
    // Plant-specific functionality and properties
    new protected void Update()
    {
        base.Update(); // Call base class's Update method
        // Implement plant-specific update logic
    }

    protected override void Grow()
    {
        // Implement plant-specific growth logic
        base.Grow(); // Call base class's method if necessary
    }
}
