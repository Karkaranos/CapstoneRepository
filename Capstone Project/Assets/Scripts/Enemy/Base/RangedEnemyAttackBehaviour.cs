/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 3/30/2026
 * Last Modified: 3/30/2026
 * Brief: Calls the player's damage function through a animation event
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;

public class RangedEnemyAttackBehaviour : MonoBehaviour
{
    private int attackDamage;

    /// <summary>
    /// Sets how much damage the attack will do
    /// </summary>
    /// <param name="amount"></param>
    public void SetDamage(int amount)
    {
        attackDamage = amount;
    }

    /// <summary>
    /// Animation event that calls the damage function
    /// </summary>
    public void DamagePlayer()
    {
        FindFirstObjectByType<PlayerStats>().TakeDamage(attackDamage, PlayerStats.DamageSource.Ranged);
    }

    /// <summary>
    /// Animation event that destroys the prefab
    /// </summary>
    public void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
