/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		Base class for all enemies
External Resources : 	
***************************************************/
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
   [field: SerializeField] public float maxHealth { get; set; } = 5f;
    public float currentHealth { get; set; }

    protected EnemyStateMachine enemyStateMachine;

    private void Awake()
    {
        enemyStateMachine = new EnemyStateMachine();
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        print("Enemy takes damage");

        if(currentHealth < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        print("Enemy is dead!");
    }
}
