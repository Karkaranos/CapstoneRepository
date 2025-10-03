/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/2/2025
 * Last Modified: 10/2/2025
 * Brief: Used to store what stat a tile is affecting and bu how much
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

[System.Serializable]
public class StatsOnTile
{
    public enum TileStats
    {
        Armor,
        Health,
        Accuracy,
        Dodge
    }

    public TileStats Stat;

    public int StatChangeValue;
}
