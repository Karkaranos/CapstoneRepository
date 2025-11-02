/*************************************************
Author Names : 	Jay Embry
Date Created : 	10/07/2025
Date Last Modified : 10/29/2025
Brief Description : Contains rune types and effects
External Resources : 	
	***************************************************/

using Mono.Cecil;
using NaughtyAttributes;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RuneEvents : MonoBehaviour
{

    #region INITIALIZATION

    public enum Prep
    {

        Visuals,
        Testing

    }

    [SerializeField] private Prep currentInspectorShowing;

    //for waiting on player input
    bool waitingForThePlayer;

    //Stores the currently using rune
    private RuneData storedData;

    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {

        PublicEvents.SelectTile += TargetSelectedTile;
        PublicEvents.RuneSelected += StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased += MasteryUnlocked;

        PublicEvents.RightClicked += CancelCasting;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.SelectTile -= TargetSelectedTile;
        PublicEvents.RuneSelected -= StoreSelectedRuneData;

        PublicEvents.MasteryRunePurchased -= MasteryUnlocked;

    }

    private bool lightningMastered;
    private bool windMastered;

    /// <summary>
    /// called whenever a rune's highest tier has been purchased
    /// </summary>
    /// <param name="runeType"> the rune's element </param>
    void MasteryUnlocked(RuneType runeType)
    {

        switch (runeType)
        {

            case (RuneType.Lightning):

                lightningMastered = true;

                break;

            case (RuneType.Wind):

                windMastered = true;

                break;

            default:

                break;

        }

        Debug.Log(runeType + " mastery unlocked!");

    }

    #endregion INITIALIZATION


    #region VISUALS

    [HorizontalLine(4, EColor.Red)]

    //for menu-swapping purposes
    [ShowIf(nameof(currentInspectorShowing), Prep.Visuals), SerializeField]
    GameObject playerMenu;

    //early testing stuff
    [ShowIf(nameof(currentInspectorShowing), Prep.Visuals), SerializeField]
    TMP_Text debugText;

    [ShowIf(nameof(currentInspectorShowing), Prep.Visuals), SerializeField]
    TMP_Text debugComboText;

    #endregion VISUALS


    #region TESTING

    [HorizontalLine(4, EColor.Yellow)]

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    //temp value for player communication
    TMP_Text temp;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    TileBehaviour tileBehaviour;


    //stores the rune that the player has most recently selected
    //they can be shown for now for testing purposes
    //ideally, these shouldn't be messed with in the future
    [Header("Test Variables")]

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    RuneType storedRuneType;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneNumber;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    float storedRuneDamage;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneRange;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    GameObject storedRuneVFX;

    [ShowIf(nameof(currentInspectorShowing), Prep.Testing), SerializeField]
    int storedRuneCost;


    //test
    [Button("Attack Tile Test")]
    public void AttackTileTest()
    {

        waitingForThePlayer = true;
        TargetSelectedTile(tileBehaviour);
        storedData = new RuneData(storedRuneType, storedRuneNumber, "Test", "Test Description", storedRuneDamage, storedRuneRange, storedRuneVFX);

    }

    #endregion TESTING


    #region RUNE EVENTS

    /// <summary>
    /// Prepares the rune that the player chooses to attack with
    /// </summary>
    /// <param name="rd"> Rune Data </param>
    public void StoreSelectedRuneData(RuneData rd)
    {

        waitingForThePlayer = true;

        storedRuneType = rd.TypeOfRune;
        storedRuneNumber = rd.NumberOnSkillTree;
        storedRuneDamage = rd.RuneDamage;
        storedRuneRange = rd.RuneRange;
        storedRuneVFX = rd.RuneVFX;
        storedRuneCost = rd.RuneActionPoints;

        storedData = rd;

        if(debugText != null)
        {

            debugText.text = "Waiting on a target...";

        }

        //to prevent softlocking FOR NOW
        //playerMenu.SetActive(true);
        //this.gameObject.SetActive(false);

    }

    /// <summary>
    /// exits attack menu when the right mouse button is clicked
    /// can be changed to something else later
    /// </summary>
    void CancelCasting()
    {

        if(waitingForThePlayer)
        {

            playerMenu.SetActive(true);
            this.gameObject.SetActive(false);

            waitingForThePlayer = false;

            if (debugText != null)
            {

                debugText.text = "";

            }

        }

    }

    /// <summary>
    /// Checks if the selected tile has an enemy in it
    /// If it does, the player's selected rune will target the enemy on the selected tile
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    public void TargetSelectedTile(TileBehaviour tile)
    {

        if (waitingForThePlayer &&
            FindFirstObjectByType<GameManager>().CurrentActionPoints >= storedRuneCost)
        {

             switch (storedRuneType)
                    {

                        case (RuneType.Lightning):

                            SelectLightningRune(tile);
                            break;

                        case (RuneType.Wind):

                            SelectWindRune(tile);
                            break;

                        default:

                            break;

                    }

        }

    }

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectLightningRune(TileBehaviour target)
    {

        int distance = Mathf.RoundToInt(Vector2.Distance(target.transform.position, GridManager.playerPosition));
        GameObject vfx;

        switch (storedRuneNumber)
        {

            //targets one opponent for moderate damage
            case (1):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    if(debugText != null)
                    {

                        debugText.text = ("Target hit for " +
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier) + " damage!");

                    }

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    //vfx.GetComponentInChildren<TextMeshPro>().text =
                        //(storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            //targets two opponents
            //one is directly targeted, and the other is the closest to the original target
            case (2):

                if (target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    FindSecondaryTarget(target);

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    //vfx.GetComponentInChildren<TextMeshPro>().text =
                        //(storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    if(secondaryTarget != null)
                    {

                        secondaryTarget.GetComponentInChildren<Enemy>().Damage
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                        CheckRuneCombination(secondaryTarget.GetComponentInChildren<Enemy>());

                        if (debugText != null)
                        {

                            debugText.text = ("Targets hit for " +
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier) + " damage!");

                        }

                        vfx = Instantiate(storedRuneVFX, secondaryTarget.transform);
                        //vfx.GetComponentInChildren<TextMeshPro>().text =
                            //(storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    }
                    else
                    {

                        if (debugText != null)
                        {

                            debugText.text = ("Target hit for " +
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier) + " damage!");

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets one opponent and all other opponents in range for less damage
            case (3):


                if (target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    int radius = 3;

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    if (debugText != null)
                    {

                        debugText.text = ("Target hit for " +
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier) + " damage!");

                    }

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    //vfx.GetComponentInChildren<TextMeshPro>().text =
                        //(storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    foreach (TileBehaviour enemy in enemies)
                    {

                        if (enemy == target)
                        {

                            continue;

                        }

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            //hardcoding this feels bad i can change this later
                            enemy.GetComponentInChildren<Enemy>().Damage
                                (15 * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                            CheckRuneCombination(enemy.GetComponentInChildren<Enemy>());

                            if (debugText != null)
                            {

                                debugText.text = "Multiple targets hit!";

                            }

                            vfx = Instantiate(storedRuneVFX, enemy.transform);
                            //vfx.GetComponentInChildren<TextMeshPro>().text =
                                //(15 * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets one opponent for a large amount of damage
            case (4):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    if (debugText != null)
                    {

                        debugText.text = ("Target hit for " +
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier) + " damage!");

                    }

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    //vfx.GetComponentInChildren<TextMeshPro>().text =
                        //(storedRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            default:

                break;
        }

        PublicEvents.RuneCast(storedRuneCost);

    }

    //variable that stores the enemy that's closest to the target
    //for lightning 2
    TileBehaviour secondaryTarget;

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    TileBehaviour FindSecondaryTarget(TileBehaviour target)
    {

        TileBehaviour[] otherEnemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

        float closestDistance = Mathf.Infinity;
        Vector3 primaryTargetPos = target.gameObject.transform.position;

        foreach(TileBehaviour potentialTarget in otherEnemies)
        {

            if(potentialTarget == target)
            {

                continue;

            }

            Vector3 dir = potentialTarget.gameObject.transform.position - primaryTargetPos;
            float distanceFromTarget = dir.sqrMagnitude;

            if(distanceFromTarget < closestDistance && potentialTarget.GetComponentInChildren<Enemy>() != null)
            {

                closestDistance = distanceFromTarget;
                secondaryTarget = potentialTarget;

            }

        }

        return secondaryTarget;

    }


    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="target"> tile that the player has selected </param>
    public void SelectWindRune(TileBehaviour target)
    {

        int distance = Mathf.RoundToInt(Vector2.Distance(target.transform.position, GridManager.playerPosition));
        GameObject vfx;
        int radius;

        switch (storedRuneNumber)
        {

            //knocks adjacent enemies backwards and damages them
            case (1):

                if ((distance / 2) <= storedRuneRange)
                {

                    radius = 2;

                    TileBehaviour[] tiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    List<TileBehaviour> enemies = tiles.ToList();

                    foreach (TileBehaviour enemy in tiles)
                    {

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            enemy.GetComponentInChildren<Enemy>().Damage
                                (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                            CheckRuneCombination(enemy.GetComponentInChildren<Enemy>());

                            if (debugText != null)
                            {

                                debugText.text = ("Target(s) hit for " +
                                (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) + " damage!");

                            }

                            vfx = Instantiate(storedRuneVFX, enemy.transform);
                            vfx.GetComponentInChildren<TextMeshPro>().text =
                                (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                            //moves enemy backwards
                            if(enemy != target)
                            {

                                SendEnemyBackwards(target, enemy, enemies);

                            }

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

            //targets an opponent for moderate damage
            //MAYBE it will target another opponent
            //will need more concrete information
            case (2):

                if(target.gameObject.GetComponentInChildren<Enemy>() != null &&
                    (distance / 2) <= storedRuneRange)
                {

                    target.GetComponentInChildren<Enemy>().Damage
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                    CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                    if (debugText != null)
                    {

                        debugText.text = ("Target hit for " +
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) + " damage!");

                    }

                    vfx = Instantiate(storedRuneVFX, target.transform);
                    vfx.GetComponentInChildren<TextMeshPro>().text =
                        (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                    EndPlayerAttackPhase();

                }

                break;

            //creates a shield on the player's tile
            case (3):

                if(target.GetComponentInChildren<PlayerBehavior>() != null)
                {

                    ShieldBehavior newShield = target.gameObject.AddComponent<ShieldBehavior>();

                    if (debugText != null)
                    {

                        debugText.text = "Shield added!";

                    }

                    newShield.OnShieldGenerated(target.transform, storedRuneVFX);

                    EndPlayerAttackPhase();

                }

                break;

            //delays target's turn and damages surrounding enemies
            case (4):

                if((distance / 2) <= storedRuneRange)
                {

                    List<TileBehaviour> validEnemies = new List<TileBehaviour>();

                    radius = 3;

                    vfx = Instantiate(storedRuneVFX, target.transform);

                    if (target.GetComponentInChildren<Enemy>() != null)
                    {

                        target.GetComponentInChildren<Enemy>().DelayedTurnStatus(true);

                        target.GetComponentInChildren<Enemy>().Damage
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);
                        CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                        if (debugText != null)
                        {

                            debugText.text = ("Target hit for " +
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) + " damage!");

                        }

                        vfx.GetComponentInChildren<TextMeshPro>().text =
                            (storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier).ToString();

                    }

                    TileBehaviour[] enemies = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);

                    foreach (TileBehaviour enemy in enemies)
                    {

                        if(enemy == target)
                        {

                            continue;

                        }

                        if ((Vector2.Distance(target.transform.position, enemy.transform.position) / 2) <= radius &&
                            enemy.GetComponentInChildren<Enemy>() != null)
                        {

                            validEnemies.Add(enemy);

                        }

                    }

                    if(validEnemies.Count > 0)
                    {

                        for (int i = 0; i < validEnemies.Count; i++)
                        {

                            validEnemies[i].GetComponentInChildren<Enemy>().Damage
                                    (Mathf.RoundToInt((storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) / validEnemies.Count));
                            CheckRuneCombination(target.GetComponentInChildren<Enemy>());

                            if (debugText != null)
                            {

                                debugText.text = "Multiple targets hit!";

                            }

                            vfx = Instantiate(storedRuneVFX, validEnemies[i].transform);
                            vfx.GetComponentInChildren<TextMeshPro>().text =
                                (Mathf.RoundToInt(storedRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier) / validEnemies.Count).ToString();

                        }

                    }

                    EndPlayerAttackPhase();

                }

                break;

        }

    }

    /// <summary>
    /// finds the tile in the opposite direction from the player adjacent to an enemy and moves them there
    /// sorry if this is a fucked way of going about this. i'm just glad this works
    /// </summary>
    /// <param name="originalTarget"> original tile that the player had targeted </param>
    /// <param name="enemy"> the enemy getting blown back </param>
    /// <param name="enemies"> the rest of the tiles with enemies on them </param>
    void SendEnemyBackwards(TileBehaviour originalTarget, TileBehaviour enemy, List<TileBehaviour> enemies)
    {

        Vector3 newTilePos;
        if(originalTarget.transform.position.z != enemy.transform.position.z)
        {

            newTilePos.x = (Mathf.Sign(enemy.transform.position.x - originalTarget.transform.position.x) + enemy.transform.position.x);
            newTilePos.z = ((Mathf.Sign(enemy.transform.position.z - originalTarget.transform.position.z) * 1.5f) + enemy.transform.position.z);

        }
        else
        {

            newTilePos.x = ((Mathf.Sign(enemy.transform.position.x - originalTarget.transform.position.x) * 2) + enemy.transform.position.x);
            newTilePos.z = enemy.transform.position.z;

        }

        newTilePos.y = 0f;

        TileBehaviour newTile = enemies.Find(x => x.transform.position == newTilePos);

        if(newTile == null)
        {

            return;

        }

        if (newTile.GetComponentInChildren<SpriteRenderer>() != null && newTile.GetComponentInChildren<Enemy>() != null)
        {

            if(newTile.GetComponentInChildren<Enemy>() != null)
            {

                SendEnemyBackwards(originalTarget, newTile, enemies);

            }
            else { return; }

        }

        Enemy movedEnemy = enemy.GetComponentInChildren<Enemy>();
        movedEnemy.gameObject.transform.parent = newTile.transform;
        movedEnemy.transform.localPosition  = new Vector3 (0, 0, 0);

    }

    /// <summary>
    /// checks if the enemy has been hit by a spell prior
    /// if so, a combo is triggered
    /// </summary>
    /// <param name="enemy"> target </param>
    void CheckRuneCombination(Enemy enemy)
    {

        if (!enemy.HasStatusEffect)
        {

            enemy.GetComponentInChildren<Enemy>().RuneStatusEffect = storedRuneType;
            enemy.GetComponentInChildren<Enemy>().RuneStatusEffectNumber = storedRuneNumber;

            enemy.HasStatusEffect = true;

            Debug.Log("Status effect added!");

        }
        else
        {

            switch (storedRuneType, enemy.RuneStatusEffect)
            {

                case (RuneType.Lightning, RuneType.Wind):

                    LightningAndWindCombo(enemy, storedRuneNumber, enemy.RuneStatusEffectNumber);
                    Debug.Log("Combo called!");

                    if (debugComboText != null)
                    {

                        debugComboText.text = "Lighting/Wind Combo!";

                    }

                    break;

                case (RuneType.Wind, RuneType.Lightning):

                    LightningAndWindCombo(enemy, enemy.RuneStatusEffectNumber, storedRuneNumber);
                    Debug.Log("Combo called!");

                    if (debugComboText != null)
                    {

                        debugComboText.text = "Wind/Lightning Combo!";

                    }

                    break;

                default:

                    break;
            }

            enemy.HasStatusEffect = false;

        }

    }

    /// <summary>
    /// calls lightning and wind combo effect
    /// </summary>
    /// <param name="enemy"> initial target </param>
    /// <param name="lightningTier"> which lightning rune was last used on this enemy </param>
    /// <param name="windTier"> which wind rune was last used on this enemy </param>
    void LightningAndWindCombo(Enemy enemy, int lightningTier, int windTier)
    {

        //PART 1: FINDING TARGETS

        //until we get actual vfx for this i'm leaving it blank because it will be sooooo cluttered
        int radius = 2;

        TileBehaviour[] tiles = FindObjectsByType<TileBehaviour>(FindObjectsSortMode.None);
        List<TileBehaviour> validEnemies = new List<TileBehaviour>();

        foreach (TileBehaviour tile in tiles)
        {

            if (tile == enemy.GetComponentInParent<TileBehaviour>())
            {

                continue;

            }

            if ((Vector2.Distance(enemy.transform.position, tile.transform.position) / 2) <= radius &&
               tile.GetComponentInChildren<Enemy>() != null)
            {

                validEnemies.Add(tile);

            }

        }


        //PART 2: LIGHTNING DAMAGE

        int lightningDamage;
        int lightningTargetDamage;

        switch (lightningTier)
        {

            case (1):

                lightningDamage = 10;

                lightningTargetDamage = 20;

                break;

            case (2):

                lightningDamage = 15;

                lightningTargetDamage = 40;

                break;

            case (3):

                lightningDamage = 15;

                lightningTargetDamage = 40;

                break;

            case (4):

                lightningDamage = 20;

                lightningTargetDamage = 60;

                break;

            default:

                lightningDamage = 0;

                lightningTargetDamage = 0;

                break;
        }

        for (int i = 0; i < validEnemies.Count; i++)
        {

            validEnemies[i].GetComponentInChildren<Enemy>().Damage
                (lightningDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);

            Debug.Log(validEnemies[i] + " took " + lightningDamage + " damage!");

        }

        if(lightningMastered)
        {

            if(enemy != null)
            {

                enemy.Damage(lightningTargetDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);

                Debug.Log(enemy + " took " + lightningTargetDamage + " damage!");

            }

            for (int i = 0; i < validEnemies.Count; i++)
            {

                if (validEnemies[i] != null)
                {

                    validEnemies[i].GetComponentInChildren<Enemy>().Damage
                    (lightningDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier);

                    Debug.Log(validEnemies[i] + " took " + lightningDamage + " damage!");

                }

            }



        }


        //PART 3: WIND DAMAGE

        int windPrimaryDamage;

        int windSecondaryDamage;

        int windTempHealth;

        switch (windTier)
        {

            case (1):

                windPrimaryDamage = 40;

                windSecondaryDamage = 10;

                windTempHealth = 10;

                break;

            case (2):

                windPrimaryDamage = 50;

                windSecondaryDamage = 15;

                windTempHealth = 20;

                break;

            case (4):

                windPrimaryDamage = 60;

                windSecondaryDamage = 20;

                windTempHealth = 30;

                break;

            default:

                windPrimaryDamage = 0;

                windSecondaryDamage = 0;

                windTempHealth = 0;

                break;
        }

        if(enemy != null)
        {

            enemy.Damage(windPrimaryDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);

            Debug.Log(enemy + " took " + windPrimaryDamage + " damage!");

        }

        for (int i = 0; i < validEnemies.Count; i++)
        {


            if (validEnemies[i] != null)
            {

                validEnemies[i].GetComponentInChildren<Enemy>().Damage
                (windSecondaryDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier);

                Debug.Log(validEnemies[i] + " took " + windSecondaryDamage + " damage!");

            }

        }

        if(windMastered)
        {

            FindFirstObjectByType<PlayerStats>().AddTempHealth(windTempHealth);

            Debug.Log("Wind mastery worked!");

        }

    }


    /// <summary>
    /// runs whenever an enemy is successfully targeted
    /// made into a function to prevent SOME clutter
    /// </summary>
    void EndPlayerAttackPhase()
    {

        waitingForThePlayer = false;

        if (PublicEvents.EnemyTurnStarted != null)
        {

            PublicEvents.EnemyTurnStarted();

        }

        playerMenu.SetActive(true);
        this.gameObject.SetActive(false);

        Invoke("ClearText", 1);

    }

    /// <summary>
    /// evil temporary code for evil temporary text
    /// clears debug text
    /// </summary>
    void ClearText()
    {

        if(debugText != null)
        {

            debugText.text = "";

        }

        if(debugComboText != null)
        {

            debugComboText.text = "";

        }

    }

    #endregion RUNE EVENTS

}
