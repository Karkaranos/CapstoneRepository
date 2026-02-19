using UnityEngine;

public class PipDisplayBehavior : MonoBehaviour
{
    public GameObject[] pipIndicators;

    public void DisplayPips(int amount) {
        if (amount > pipIndicators.Length) 
        {
            amount = pipIndicators.Length;
        }
        for (int i = 0; i < amount; i++) {
            pipIndicators[pipIndicators.Length - i].gameObject.SetActive(false);
        }
    }
}
