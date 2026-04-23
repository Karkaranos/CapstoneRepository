/******************************************************************************
 * Author: Brad Dixon, Cade Naylor, Aidan Ratcliffe
 * Creation Date: 3/5/2026
 * Last Modified: 4/16/2026
 * Brief: Damages an entity if the step into the hazard or end their turn on a hazard
 * External Resources; N/A
 * ***************************************************************************/
using UnityEngine;

public class DamageHazardBehaviour : MonoBehaviour
{
    public bool canDamage;
    [SerializeField] private int hazardDamage;
    private Animator anim;
    private ParticleSystem bushParticles;

    /// <summary>
    /// Grabs a reference to the animator component & particle system component
    /// </summary>
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        bushParticles = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Damages an entity when they walk onto the hazard, activates particles on trigger
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.GetComponent<PlayerBehavior>()|| collision.gameObject.GetComponent<Enemy>())
        {
            canDamage = true;
            GetComponentInParent<TileBehaviour>().DamageEntity(hazardDamage, false);

            anim.SetTrigger("SteppedOn");
            bushParticles.Play();
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
        GetComponentInParent<TileBehaviour>().DamageEntity(hazardDamage, false);

        anim.SetTrigger("SteppedOn");
    }
}
