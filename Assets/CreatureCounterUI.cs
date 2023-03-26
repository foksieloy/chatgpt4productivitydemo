using TMPro;
using UnityEngine;

public class CreatureCounterUI : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        int foodCreatureCount = GameObject.FindGameObjectsWithTag("FoodCreature").Length;
        int grazingCreatureCount = GameObject.FindGameObjectsWithTag("GrazingCreature").Length;
        int hunterCreatureCount = GameObject.FindGameObjectsWithTag("HunterCreature").Length;

        textMeshPro.text = $"Food Creatures: {foodCreatureCount}\nGrazing Creatures: {grazingCreatureCount}\nHunter Creatures: {hunterCreatureCount}";
    }

}
