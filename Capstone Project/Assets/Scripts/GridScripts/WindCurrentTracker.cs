/*************************************************
Author Names : 	Jay Embry
Date Created : 	02/15/2026
Date Last Modified : 02/15/2026
Brief Description : Create/manages wind currents
External Resources : 	
	***************************************************/

using UnityEngine;
using System.Collections.Generic;

public class WindCurrentTracker : MonoBehaviour
{

    //tiles (and their vfx) from the path selected prior
    [HideInInspector] public List<TileBehaviour> WindCurrentTiles = new List<TileBehaviour>();
    [HideInInspector] public List<GameObject> WindCurrentVFX = new List<GameObject>();

    //damage caused by currents + knockback
    [HideInInspector] public float CurrentDamage;
    [HideInInspector] public float CurrentKBDamage;

    /// <summary>
    /// spawns vfx
    /// </summary>
    /// <param name="vfx"> wind 2b visual effect </param>
    public void GenerateWindCurrent(GameObject vfx)
    {

        foreach(TileBehaviour tile in WindCurrentTiles)
        {

            WindCurrentVFX.Add(Instantiate(vfx, tile.transform));

        }

    }

    /// <summary>
    /// sends enemy down wind current either until they hit another enemy or reach the end of the line
    /// if they don't reach the end of the line, the enemy that they did will
    /// </summary>
    /// <param name="startingTile"> tile that the was standing on/moved into </param>
    /// <param name="enemy"> enemy hit by current </param>
    public void SendThroughWindCurrent(int startingTile, Enemy enemy)
    {

        for(int i = startingTile; i < WindCurrentTiles.Count; i++)
        {

            if(i < WindCurrentTiles.Count - 1)
            {

                if (WindCurrentTiles[i + 1].GetComponentInChildren<Enemy>() && CanMoveBackwards(WindCurrentTiles[i], WindCurrentTiles[i+1]))
                {

                    SendEnemyBackwards(WindCurrentTiles[i], WindCurrentTiles[i + 1], WindCurrentTiles[i + 1].GetComponentInChildren<Enemy>());

                }

                enemy.transform.SetParent(WindCurrentTiles[i + 1].transform);

                enemy.transform.position = new Vector3(WindCurrentTiles[i + 1].transform.position.x, 0, WindCurrentTiles[i + 1].transform.position.z);

                GridManager.MoveToTile(WindCurrentTiles[i].IndexInGrid, WindCurrentTiles[i + 1].IndexInGrid, -2);

                enemy.GetComponent<GridPathfinding>().SetPosition(WindCurrentTiles[i + 1].IndexInGrid);

            }
            else
            {

                SendEnemyBackwards(WindCurrentTiles[i - 1], WindCurrentTiles[i], enemy);

            }

        }

    }

    /// <summary>
    /// checks to see if an enemy can be knocked back
    /// </summary>
    /// <param name="originTile"> where the "hit" came from </param>
    /// <param name="enemyTile"> the enemy being "hit" </param>
    /// <returns> tile status </returns>
    public static bool CanMoveBackwards(TileBehaviour originTile, TileBehaviour enemyTile)
    {

        Vector2Int newTilePos = enemyTile.IndexInGrid;

        if (originTile.IndexInGrid.x < enemyTile.IndexInGrid.x)
        {

            newTilePos.x += 1;

        }
        else if (originTile.IndexInGrid.x > enemyTile.IndexInGrid.x)
        {

            newTilePos.x -= 1;

        }

        if (originTile.IndexInGrid.y < enemyTile.IndexInGrid.y)
        {

            newTilePos.y += 1;

        }
        else if (originTile.IndexInGrid.y > enemyTile.IndexInGrid.y)
        {

            newTilePos.y -= 1;

        }

        if (GridManager.combatGrid[newTilePos.x, newTilePos.y])
        {

            TileBehaviour newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

            if (newTile.GetComponentInChildren<Enemy>())
            {

                if (CanMoveBackwards(enemyTile, newTile))
                {

                    return newTile.entityOnGrid == -1 || newTile.entityOnGrid == -2;

                }
                else
                {

                    return false;

                }

            }
            else
            {

                return newTile.entityOnGrid == -1 || newTile.entityOnGrid == -2;

            }

        }
        else
        {

            return false;

        }

    }

    /// <summary>
    /// sends "hit" enemy backwards
    /// </summary>
    /// <param name="originTile"> where the enemy is being "hit" from </param>
    /// <param name="enemyTile"> where the enemy is "hit" </param>
    /// <param name="enemy"> the enemy that is "hit" </param>
    void SendEnemyBackwards(TileBehaviour originTile, TileBehaviour enemyTile, Enemy enemy)
    {

        Vector2Int newTilePos = enemyTile.IndexInGrid;

        if (originTile.IndexInGrid.x < enemyTile.IndexInGrid.x)
        {

            newTilePos.x += 1;

        }
        else if (originTile.IndexInGrid.x > enemyTile.IndexInGrid.x)
        {

            newTilePos.x -= 1;

        }

        if (originTile.IndexInGrid.y < enemyTile.IndexInGrid.y)
        {

            newTilePos.y += 1;

        }
        else if (originTile.IndexInGrid.y > enemyTile.IndexInGrid.y)
        {

            newTilePos.y -= 1;

        }

        if (GridManager.combatGrid[newTilePos.x, newTilePos.y])
        {

            TileBehaviour newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

            if (newTile.entityOnGrid == -1)
            {

                enemy.transform.SetParent(newTile.transform);

                enemy.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                GridManager.MoveToTile(enemyTile.IndexInGrid, newTilePos, -2);

                enemy.GetComponent<GridPathfinding>().SetPosition(newTilePos);

            }
            else if (newTile.entityOnGrid == -2)
            {

                if (CanMoveBackwards(enemyTile, newTile))
                {

                    newTile.GetComponentInChildren<Enemy>().Damage(CurrentKBDamage, Enemy.DamageType.Wind);

                    SendEnemyBackwards(enemyTile, newTile, newTile.GetComponentInChildren<Enemy>());

                    enemy.transform.SetParent(newTile.transform);

                    enemy.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                    GridManager.MoveToTile(enemyTile.IndexInGrid, newTilePos, -2);

                    enemy.GetComponent<GridPathfinding>().SetPosition(newTilePos);

                }

            }

        }

    }

    /// <summary>
    /// destroys wind current
    /// </summary>
    public void DestroyCurrents()
    {

        for(int i = 0; i < WindCurrentVFX.Count; i++)
        {

            Destroy(WindCurrentVFX[i]);

        }

        Destroy(this);

    }
    
}
