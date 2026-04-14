/******************************************************************************
 * Author: Brad Dixon, Cade Naylor
 * Creation Date: 3/5/2026
 * Last Modified: 4/9/2026
 * Brief: Damages an entity if the step into the hazard or end their turn on a hazard
 * External Resources; N/A
 * ***************************************************************************/
using UnityEngine;

public class DamageHazardBehaviour : MonoBehaviour
{
    public bool canDamage;
    [SerializeField] private int hazardDamage;
    private Animator anim;

    /// <summary>
    /// Grabs a reference to the animator component
    /// </summary>
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();    
    }

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

            anim.SetTrigger("SteppedOn");
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

        anim.SetTrigger("SteppedOn");
    }
}
