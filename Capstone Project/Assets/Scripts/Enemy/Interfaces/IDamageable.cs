/*************************************************
Author Names : 		Clare Grady, 
Date Created : 		09/30/2025
Date Last Modified : 	09/30/2025
Brief Description : 		Basic interface for 
				damageble objects / creatures 
External Resources : 	
***************************************************/

using UnityEngine;

public interface IDamageable
{
	float maxHealth { get; set; }
	float currentHealth { get; set; }
	public void Die();
	public void Damage(float damage); 
}
 