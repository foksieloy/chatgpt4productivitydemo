using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureCounterUI : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private readonly Dictionary<string, int> creatureCounts = new();

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdateCreatureCounts();

        string countText = "";

        foreach (KeyValuePair<string, int> entry in creatureCounts)
        {
            countText += entry.Key + ": " + entry.Value + "\n";
        }

        textMeshPro.text = countText;
    }

    private void UpdateCreatureCounts()
    {
        // Reset the counts
        creatureCounts.Clear();

        // Find all objects that are children of Creature
        CreatureController[] creatures = FindObjectsOfType<CreatureController>();

        // Iterate through the creatures and update the count for each creature type
        foreach (CreatureController creature in creatures)
        {
            string typeName = creature.GetType().Name;

            if (creatureCounts.ContainsKey(typeName))
            {
                creatureCounts[typeName]++;
            }
            else
            {
                creatureCounts[typeName] = 1;
            }
        }
    }

}
