using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WindCurrentTracker : MonoBehaviour
{

    GameObject current;

    [HideInInspector] public TileBehaviour WindCurrentOrigin;
    [HideInInspector] public List<TileBehaviour> WindCurrentTiles = new List<TileBehaviour>();
    [HideInInspector] public List<GameObject> WindCurrentVFX = new List<GameObject>();

    [HideInInspector] public float CurrentDamage;
    [HideInInspector] public float CurrentKBDamage;

    public void GenerateWindCurrent(GameObject vfx)
    {

        foreach(TileBehaviour tile in WindCurrentTiles)
        {

            WindCurrentVFX.Add(Instantiate(vfx, tile.transform));

        }

    }

    public void SendThroughWindCurrent(int startingTile, Enemy enemy)
    {

        for(int i = startingTile; i < WindCurrentTiles.Count; i++)
        {

            if(i == 0)
            {

                if(CanMoveBackwards(WindCurrentOrigin, WindCurrentTiles[i]))
                {

                    SendEnemyBackwards(WindCurrentOrigin, WindCurrentTiles[i], enemy);

                }

            }
            else
            {

                if(CanMoveBackwards(WindCurrentTiles[i - 1], WindCurrentTiles[i]))
                {

                    SendEnemyBackwards(WindCurrentTiles[i - 1], WindCurrentTiles[i], enemy);

                }

            }

        }

    }

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

    public void DestroyCurrents()
    {

        for(int i = 0; i < WindCurrentVFX.Count; i++)
        {

            Destroy(WindCurrentVFX[i]);

        }

        Destroy(this);

    }
    
}
