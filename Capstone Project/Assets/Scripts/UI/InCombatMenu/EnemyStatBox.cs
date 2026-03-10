/*************************************************
Author Names : 		Clare
Date Created : 		3/5/2026
Date Last Modified : 3/5/2026
Brief Description : enemy stat box controler + information 
***************************************************/
using TMPro;
using UnityEngine;

public class EnemyStatBox : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text damage;
    [SerializeField] private TMP_Text range;
    [SerializeField] private TMP_Text movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup.alpha = 0.0f;
    }

    private void OnEnable()
    {
        PublicEvents.DisplayEnemyStatbox += DisplayStats;
        PublicEvents.HideEnemyStatbox += HideStats;
    }

    private void OnDisable()
    {
       PublicEvents.DisplayEnemyStatbox -= DisplayStats;
       PublicEvents.HideEnemyStatbox -= HideStats;
    }


    private void DisplayStats(Enemy enemy)
    {
        health.text = "Health: " + enemy.currentHealth;
        damage.text = "Damage: " + enemy.damage;
        movement.text = "Movement: " + enemy.GetMovementSpeed(); 

        if(enemy.isRangedEnemy)
        {
            RangedEnemy rangedEnemy = enemy.gameObject.GetComponent<RangedEnemy>();
            range.text = "Range: " + rangedEnemy.minimumAttackDistance + " - " + rangedEnemy.maxAttackDistance + " tiles";
        }
        else
        {
            range.text = "Range: " + enemy.attackRange + " tiles";
        }

        canvasGroup.alpha = 1; 
    }

    public void HideStats()
    {
        canvasGroup.alpha = 0;
    }
}
