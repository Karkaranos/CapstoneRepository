/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/5/2025
Brief Description : 	Helper class for Artifacts                     
External Resources : 	N/A
***************************************************/
using UnityEngine;
using NaughtyAttributes;
[System.Serializable]
public class ArtifactEffects
{
    [AllowNesting] public Effects Effect;
    [Tooltip("How much the selected stat is changed. Can be positive or negative"), AllowNesting] public float StatChangeAmount;
}