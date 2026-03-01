/*************************************************
Author Names : 		Clare
Date Created : 		2/28/2026
Date Last Modified : 2/28/2026
Brief Description : controls the out of combat menu stat box
***************************************************/
using TMPro;
using UnityEngine;

public class StatBox : MonoBehaviour
{
    [SerializeField] private TMP_Text windDamange;
    [SerializeField] private TMP_Text lightningDamage;
    [SerializeField] private TMP_Text meleeResist;
    [SerializeField] private TMP_Text rangeResist;


    private void OnEnable()
    {
        PublicEvents.AddToStatBox += AddToStatBox;    
    }

    private void OnDisable()
    {
        PublicEvents.AddToStatBox -= AddToStatBox;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddToStatBox(ArtifactData data)
    {
        string name = data.Name;
        switch(name)
        {
            case "Wind +":
                string previousInfo = windDamange.text.Split(':', System.StringSplitOptions.None)[0];
                windDamange.text = previousInfo + " 20%";
                break;

            case "Lightning +":
                break;
            case "Ranged Resist":
                break;
            case "Melee Resist":
                break;
            default:
                break;

        }
    }
}
