/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/22/2025
Brief Description : 	Helper class for Artifacts                     
External Resources : 	N/A
***************************************************/
using UnityEngine;
using NaughtyAttributes;
[System.Serializable]
public class ArtifactEffects
{
    [AllowNesting] public Effects Effect;
    [Tooltip("How much the selected stat is changed. To increase, a value should be greater than 1. For vampiric, do the raw percentage (.5, etc)"), AllowNesting] public float StatChangeAmount;
    [Tooltip("Chance for this effect to trigger"), AllowNesting, Range(0f, 1f)] public float TriggerChance;
}