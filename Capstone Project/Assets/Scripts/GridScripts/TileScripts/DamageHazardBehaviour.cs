/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/5/2026
 * Last Modified: 3/5/2026
 * Brief: Damages an entity if the step into the hazard or end their turn on a hazard
 * External Resources; N/A
 * ***************************************************************************/
using UnityEngine;

public class DamageHazardBehaviour : MonoBehaviour
{
    public bool canDamage;
    [SerializeField] private int hazardDamage;

    /// <summary>
    /// Damages an entity when they walk onto the hazard
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.GetComponent<PlayerBehavior>()|| collision.gameObject.GetComponent<Enemy>())
        {
            canDamage = true;
            GetComponentInParent<TileBehaviour>().DamageEntity(hazardDamage);
        }
    }

    /// <summary>
    /// Lets the hazard know that there is no longer an entity on it
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<PlayerBehavior>()|| collision.gameObject.GetComponent<Enemy>())
        {
            canDamage = false;
        }
    }

    /// <summary>
    /// Calls the damage function in the TileBehaviour script
    /// </summary>
    public void EndTurnDamage()
    {
        GetComponentInParent<TileBehaviour>().DamageEntity(hazardDamage);
    }
}
