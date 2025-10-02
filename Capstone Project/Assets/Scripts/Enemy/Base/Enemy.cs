/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		10/1/2025
Date Last Modified : 	10/1/2025
Brief Description : 		Base class for all enemies
External Resources : 	
***************************************************/
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected float maxHealth;
    [SerializeField] protected float currentHealth = 5f;
    [SerializeField] protected float lowHealthPercentage = 0.20f; 

    //Common Enemy Variables
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
