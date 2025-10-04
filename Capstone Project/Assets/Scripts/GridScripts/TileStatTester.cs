/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 10/2/2025
 * Last Modified: 10/2/2025
 * Brief: Temporary script only used to test if stat changes can be called
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class TileStatTester : MonoBehaviour
{
    /// <summary>
    /// Gets the terrain this entity is on and shows in the console how that terrain affects it
    /// </summary>
    public void DisplayStatChange()
    {
        TileBehaviour myTile = GetComponentInParent<TileBehaviour>();

        foreach (StatsOnTile s in myTile.tileStatAffects)
        {
            if (s.StatChangeValue > 0)
            {
                Debug.Log("My " + s.Stat.ToString() + " stat is being increased by " + s.StatChangeValue + " points.");
            }
            else if (s.StatChangeValue < 0)
            {
                Debug.Log("My " + s.Stat.ToString() + " stat is being decreased by " + Mathf.Abs(s.StatChangeValue) + " points.");
            }
        }
    }
}
