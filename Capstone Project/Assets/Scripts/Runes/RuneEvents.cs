/*************************************************
Author Names : 	Jay Embry, Brad Dixon, Aidan Ratcliffe
Date Created : 	10/07/2025
Date Last Modified : 04/07/2026 (Jay Embry)
Brief Description : Contains rune types and effects
                    I promise that I'll clean this up sometime soon. I'm so sorry
External Resources : 	
	***************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using static Unity.Collections.Unicode;
using FMOD.Studio;
using FMODUnity;
using EventReference = FMODUnity.EventReference;
using Unity.VisualScripting;


public class RuneEvents : MonoBehaviour
{

    #region SETUP


    public enum Variables
    {

        ComboVariables,
        Audio,
        Animations

    }

    [SerializeField] private Variables currentInspectorShowing;

    List<TileBehaviour> targetedTiles;

    //for pathing certain attacks
    RuneData selectedRune;
    Vector2Int originalSelectedTile;
    [HideInInspector] public Vector2Int selectedTile;
    Enemy selectedEnemy;

    Vector3 ghostPos;

    int movementLeft;
    int movementUsed;

    List<Vector3> movementPos = new List<Vector3>();
    //forgot why i made this public tbh
    [HideInInspector] public List<Vector2Int> PreviousPos = new List<Vector2Int>();


    public bool Casting = false;
    public bool WaitingOnPath = false;
    public bool Pathing = false;

    #endregion SETUP



    #region AUDIO

    [HorizontalLine(4, EColor.Orange)]

    //Event reference for sound
    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_1;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_2;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_3;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference lightningSpellSFX_4;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_1;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_2;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_3;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private EventReference windSpellSFX_4;

    [ShowIf(nameof(currentInspectorShowing), Variables.Audio), SerializeField]
    private GameObject audioListenerObject;

    #endregion AUDIO



    #region ANIMATIONS

    /// <summary>
    /// holds animators for book & player
    /// </summary>
    private Variables Animations;

    [ShowIf(nameof(currentInspectorShowing), Variables.Animations), SerializeField]
    private GameObject PlayerVisual;
    private GameObject BookVisual;
    private Animator anim;
    private Animator bookanim;


    #endregion ANIMATIONS



    #region INITIALIZATION

    /// <summary>
    /// references the player animator, allowing it to be called it in another script
    /// </summary>
    /// <param name="animator"></param>
    public void AssignAnim(Animator animator)
    {
        anim = animator;
    }

    /// <summary>
    /// references the book animator, allowing it to be called it in another script
    /// </summary>
    /// <param name="animator"></param>
    public void AssignBookAnim(Animator animator)
    {
        bookanim = animator;
    }


    /// <summary>
    /// Runs whenever this script is loaded into a scene
    /// </summary>
    private void OnEnable()
    {
        //anim = PlayerVisual.GetComponent<Animator>();

        PublicEvents.LightningCast += SelectedLightningRuneCast;
        PublicEvents.WindCast += SelectedWindRuneCast;

        PublicEvents.MovementDirection += MoveDirection;

        PublicEvents.MasteryRunePurchased += MasteryUnlocked;

    }

    /// <summary>
    /// Runs whenever this script is destroyed
    /// </summary>
    private void OnDisable()
    {

        PublicEvents.LightningCast -= SelectedLightningRuneCast;
        PublicEvents.WindCast -= SelectedWindRuneCast;

        PublicEvents.MovementDirection -= MoveDirection;

        PublicEvents.MasteryRunePurchased -= MasteryUnlocked;

    }

    #endregion INITIALIZATION



    #region OTHER

    private bool lightningMastered;
    private bool windMastered;
    /// <summary>
    /// indicates when the third tier of a spell has been unlocked
    /// </summary>
    /// <param name="runeType"> type of rune unlocked </param>
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


    }

    public void GetTargets(List<TileBehaviour> tilesInRange)
    {

        targetedTiles = tilesInRange;

    }

    #endregion OTHER



    #region LIGHTNING FUNCTIONS

    /// <summary>
    /// Calls lightning rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public async void SelectedLightningRuneCast(RuneData rune, TileBehaviour tile, Enemy enemy, PlayerBehavior player)
    {
        //this should hopefully keep the player from spamming spells
        if (Casting)
        {
            return;
        }

        if (anim == null)
        {
            anim = FindFirstObjectByType<PlayerBehavior>().gameObject.GetComponentInChildren<Animator>();
        }

        if (bookanim == null)
        {
            bookanim = FindAnyObjectByType<PlayerBehavior>().gameObject.GetComponentInChildren<Animator>();
        }

        float damageDealt = Mathf.CeilToInt(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
        * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        Vector2Int playerOriginalTile = FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<TileBehaviour>().IndexInGrid;

        switch (rune.NumberOnSkillTree)
        {
            //targets a tile and  electrifies the tiles around it
            case (1):

                Casting = true;

                if (enemy != null)
                {
                    await Task.Delay(1200);
                    enemy.Damage(damageDealt, Enemy.DamageType.Lightning);
                }

                FindAdjacentTiles(tile);

                foreach(TileBehaviour adjacentTile in secondaryTargets)
                {

                    if(adjacentTile.GetComponentInChildren<Enemy>() != null && adjacentTile != tile)
                    {

                        adjacentTile.GetComponentInChildren<Enemy>().Damage(Mathf.CeilToInt(rune.SecondaryRuneDamage * FindFirstObjectByType<PlayerStats>()
                        .LightningAttackMultiplier * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier), Enemy.DamageType.Lightning);

                        adjacentTile.ElectrifyAdTiles();

                    }

                }

                tile.ElectrifyAdTiles();
                anim.SetBool("Attack", true);
                bookanim.SetBool("LAtk", true);
                bookanim.SetBool("Idle", false);
                anim.SetBool("Idle", false);
                AudioManager.instance.CreateEventInstance(lightningSpellSFX_1);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_1, audioListenerObject.transform.position);
                
                Instantiate(rune.RuneVFX, tile.transform);

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                StartCoroutine(UpdatePlayerStatus());

                break;

            //targets opponents in a cross pattern
            case (2):

                Casting = true;

                anim.SetBool("Attack", true);
                bookanim.SetBool("LAtk", true);
                bookanim.SetBool("Idle", false);
                anim.SetBool("Idle", false);
                AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);

                Instantiate(rune.RuneVFX, tile.transform);
                tile.Invoke("ElectrifyAdTiles", 1.2f);

                if(tile.GetComponentInChildren<Enemy>())
                {

                    tile.GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Lightning);

                }

                await Task.Delay(1000);

                FindLinesOfTargets(rune, tile);

                List<TileBehaviour> waveOfTiles = new List<TileBehaviour>();

                for (int i = 1; i <= rune.RuneRange; i++)
                {

                    foreach(TileBehaviour target in secondaryTargets)
                    {

                        if(Mathf.Abs(tile.IndexInGrid.x - target.IndexInGrid.x) == i || 
                        Mathf.Abs(tile.IndexInGrid.y - target.IndexInGrid.y) == i)
                        {

                            waveOfTiles.Add(target);

                        }

                    }

                    foreach(TileBehaviour target in waveOfTiles)
                    {

                        if(target.GetComponentInChildren<PlayerBehavior>() != null || target == tile)
                        {
                            continue;
                        }

                        
                        AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                        AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);

                        Instantiate(rune.RuneVFX, target.transform);

                        if(target.GetComponentInChildren<Enemy>() != null)
                        {

                            SubtractFromDamage(rune, tile, target);
                            target.GetComponentInChildren<Enemy>().Damage(damageDealt - subtraction, Enemy.DamageType.Lightning);

                        }

                        target.Invoke("ElectrifyAdTiles", 1.2f);

                    }

                    waveOfTiles.Clear();

                    await Task.Delay(1000);

                }

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                StartCoroutine(UpdatePlayerStatus());

                break;

            //teleports the player, damages adjacent enemies, and knocks enemies backwards
            case (3):

                Casting = true;

                Instantiate(rune.RuneVFX, tile.transform);

                FindFirstObjectByType<PlayerBehavior>().gameObject.transform.SetParent(tile.transform);
                FindFirstObjectByType<PlayerBehavior>().gameObject.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
                GridManager.MoveToTile(playerOriginalTile, tile.IndexInGrid, -3);

                Invoke("PlayerTeleport", .1f);

                tile.Invoke("ElectrifyAdTiles", 1.2f);

                FindAdjacentTiles(tile);

                foreach (TileBehaviour adjacentTile in secondaryTargets)
                {

                    if (adjacentTile.GetComponentInChildren<Enemy>() != null && 
                    CanMoveBackwards(FindFirstObjectByType<PlayerBehavior>().GetComponentInParent<TileBehaviour>(), adjacentTile))
                    {

                        await Task.Delay(1200);

                        adjacentTile.GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Lightning);

                        SendEnemyBackwards(GridManager.combatGrid[GridManager.playerPosition.x, GridManager.playerPosition.y], 
                        adjacentTile, adjacentTile.GetComponentInChildren<Enemy>());

                    }

                }

                anim.SetBool("Attack", true);
                bookanim.SetBool("LAtk", true);
                bookanim.SetBool("Idle", false);
                anim.SetBool("Idle", false);
                AudioManager.instance.CreateEventInstance(lightningSpellSFX_3);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_3, audioListenerObject.transform.position);

                gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);
                StartCoroutine(UpdatePlayerStatus());

                break;

            //teleports the player, damaging all enemies in their path
            case (4):

                if (!WaitingOnPath)
                {

                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    originalSelectedTile = tile.IndexInGrid;
                    selectedTile = originalSelectedTile;

                    PreviousPos.Add(selectedTile);

                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor
                    (GetComponent<RuneRangeAndTargeting>().LightningSecondaryHighlight);

                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    Pathing = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);

                }
                else if (tile == GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y]
                && WaitingOnPath)
                {

                    if (tile == GridManager.combatGrid[originalSelectedTile.x, originalSelectedTile.y] || tile.GetComponentInChildren<Enemy>())
                    {
                        FindFirstObjectByType<ButtonManager>().confirmCanvas.SetActive(true);
                        return;
                    }

                    Casting = true;

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    anim.SetBool("Attack", true);
                    bookanim.SetBool("LAtk", true);
                    bookanim.SetBool("Idle", false);
                    anim.SetBool("Idle", false);
                    AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                    AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    Pathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                }

                break;

            default:
                break;
        }
    }

    //variable that stores targets for an aoe attack
    List<TileBehaviour> secondaryTargets = new List<TileBehaviour>();

    /// <summary>
    /// called when lightning 3 is cast
    /// finds opponents in a straight line relative to the player's position and the initial target
    /// ngl i don't think that i'm seeing the light of heaven for this but it works
    /// </summary>
    /// <param name="initialTarget"> initial target that the player had picked out </param>
    /// <returns> a list of tiles for the spell to target </returns>
    List<TileBehaviour> FindLinesOfTargets(RuneData rune, TileBehaviour initialTarget = null)
    {

        secondaryTargets.Clear();

        foreach (TileBehaviour tile in GridManager.combatGrid)
        {

            if(initialTarget.transform.position.x == tile.transform.position.x && 
            Mathf.Abs(initialTarget.transform.position.x - tile.transform.position.x) <= rune.RuneRange)
            {

                secondaryTargets.Add(tile);

            }

            if(initialTarget.transform.position.z == tile.transform.position.z &&
            Mathf.Abs(initialTarget.transform.position.z - tile.transform.position.z) <= rune.RuneRange)
            {

                secondaryTargets.Add(tile);

            }

        }

        return secondaryTargets;

    }

    /// <summary>
    /// called when lightning 1 is cast
    /// finds opponents adjacent and diagonal to the initial target
    /// </summary>
    /// <param name="target"> initial target </param>
    /// <returns> a list of the tiles adjacent and diagonal to the target </returns>
    List<TileBehaviour> FindAdjacentTiles(TileBehaviour target)
    {

        secondaryTargets.Clear();

        foreach(TileBehaviour tile in GridManager.combatGrid)
        {

            if (Mathf.Abs(tile.IndexInGrid.x - target.IndexInGrid.x) <= 1 &&
            Mathf.Abs(tile.IndexInGrid.y - target.IndexInGrid.y) <= 1 && !tile.GetComponentInChildren<PlayerBehavior>())
            {

                secondaryTargets.Add(tile);

            }

        }

        return secondaryTargets;

    }

    //for lightning 2a
    float subtraction;

    float SubtractFromDamage(RuneData rune, TileBehaviour originalTarget, TileBehaviour nextTarget)
    {

        if(originalTarget.transform.position.x != nextTarget.transform.position.x)
        {

            subtraction = Mathf.CeilToInt((Mathf.Abs(originalTarget.transform.position.x - nextTarget.transform.position.x) * (rune.RuneDamage * .2f)));

        }
        else if(originalTarget.transform.position.z != nextTarget.transform.position.z)
        {

            subtraction = Mathf.CeilToInt((Mathf.Abs(originalTarget.transform.position.z - nextTarget.transform.position.z) * (rune.RuneDamage * .2f)));

        }
        else
        {

            subtraction = 0;

        }

        return subtraction;

    }

    #endregion LIGHTNING FUNCTIONS



    #region WIND FUNCTIONS

    /// <summary>
    /// Calls wind rune effect
    /// </summary>
    /// <param name="tile"> tile that the player has selected </param>
    /// <param name="enemy"> enemy that the player has selected </param>
    /// <param name="player"> when the player has selected themself </param>
    public async void SelectedWindRuneCast(RuneData rune, TileBehaviour tile, Enemy enemy = null, PlayerBehavior player = null)
    {
        if(Casting)
        {
            return;
        }

        float damageDealt = Mathf.Ceil(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
        * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        switch (rune.NumberOnSkillTree)
        {

            //knocks an enemy back, as well as an enemy in their path as is chosen by the player
            case (1):

                if(!WaitingOnPath)
                {

                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    originalSelectedTile = tile.IndexInGrid;
                    selectedTile = originalSelectedTile;
                    if(enemy != null)
                    {
                        selectedEnemy = enemy;
                    }
                    else { selectedEnemy = tile.GetComponentInChildren<Enemy>(); }

                    PreviousPos.Add(selectedTile);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    Pathing = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);


                }
                else if (tile == GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y] 
                && WaitingOnPath)
                {

                    if(tile == GridManager.combatGrid[originalSelectedTile.x, originalSelectedTile.y])
                    {
                        FindFirstObjectByType<ButtonManager>().confirmCanvas.SetActive(true);
                        return;
                    }

                    Casting = true;

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    Pathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                    await Task.Delay(400);
                    selectedEnemy.Damage(damageDealt, Enemy.DamageType.Wind);

                    anim.SetBool("Attack", true);
                    bookanim.SetBool("WAtk", true);
                    bookanim.SetBool("Idle", false);
                    anim.SetBool("Idle", false);
                    AudioManager.instance.CreateEventInstance(windSpellSFX_1);
                    AudioManager.instance.PlayOneShot(windSpellSFX_1, audioListenerObject.transform.position);

                }

                    break;

            //creates a barrier along a player's selected path
            case (2):

                if (!WaitingOnPath)
                {
                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    selectedTile = tile.IndexInGrid;

                    PreviousPos.Add(selectedTile);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    Pathing = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);
                    movementPos.Add(ghostPos);

                }
                else if (tile == GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y]
                && WaitingOnPath)
                {
                    
                    if (tile == GridManager.combatGrid[originalSelectedTile.x, originalSelectedTile.y])
                    {
                        FindFirstObjectByType<ButtonManager>().confirmCanvas.SetActive(true);
                        return;
                    }

                    Casting = true;

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    Pathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                    anim.SetBool("Attack", true);
                    bookanim.SetBool("WAtk", true);
                    bookanim.SetBool("Idle", false);
                    anim.SetBool("Idle", false);
                    AudioManager.instance.CreateEventInstance(windSpellSFX_3);
                    AudioManager.instance.PlayOneShot(windSpellSFX_3, audioListenerObject.transform.position);

                }

                    break;

            //creates a wind current along the player's selected path that damages enemies + knocks enemies backwards
            case (3):

                if (!WaitingOnPath)
                {

                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    originalSelectedTile = tile.IndexInGrid;
                    selectedTile = originalSelectedTile;

                    PreviousPos.Add(selectedTile);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    Pathing = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange + 1;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);
                    movementPos.Add(ghostPos);


                }
                else if (tile == GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y] 
                && WaitingOnPath)
                {

                    if (tile == GridManager.combatGrid[originalSelectedTile.x, originalSelectedTile.y])
                    {
                        FindFirstObjectByType<ButtonManager>().confirmCanvas.SetActive(true);
                        return;
                    }

                    Casting = true;

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    Pathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                    anim.SetBool("Attack", true);
                    bookanim.SetBool("WAtk", true);
                    bookanim.SetBool("Idle", false);
                    anim.SetBool("Idle", false);
                    AudioManager.instance.CreateEventInstance(windSpellSFX_2);
                    AudioManager.instance.PlayOneShot(windSpellSFX_2, audioListenerObject.transform.position);

                }

                break;

            //damages enemies in path, moves them around, and pulls in surrounding enemies by a little
            case (4):

                if (!WaitingOnPath)
                {

                    GridManager.RemoveHighlight();

                    selectedRune = rune;
                    originalSelectedTile = tile.IndexInGrid;
                    selectedTile = originalSelectedTile;
                    if (enemy != null)
                    {
                        selectedEnemy = enemy;
                    }
                    else { selectedEnemy = tile.GetComponentInChildren<Enemy>(); }

                    PreviousPos.Add(selectedTile);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                    GridManager.combatGrid[selectedTile.x, selectedTile.y].ShowHighlight(true);

                    WaitingOnPath = true;
                    Pathing = true;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = true;

                    movementLeft = rune.RuneRange + 1;

                    ghostPos = new Vector3(selectedTile.x, 0, selectedTile.y);
                    movementPos.Add(ghostPos);

                }
                else if (tile == GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y]
                && WaitingOnPath)
                {

                    if (tile == GridManager.combatGrid[originalSelectedTile.x, originalSelectedTile.y])
                    {
                        FindFirstObjectByType<ButtonManager>().confirmCanvas.SetActive(true);
                        return;
                    }

                    Casting = true;

                    gameObject.GetComponent<RuneRangeAndTargeting>().SetCastStatus(true);

                    anim.SetBool("Attack", true);
                    bookanim.SetBool("WAtk", true);
                    bookanim.SetBool("Idle", false);
                    anim.SetBool("Idle", false);
                    AudioManager.instance.CreateEventInstance(windSpellSFX_4);
                    AudioManager.instance.PlayOneShot(windSpellSFX_4, audioListenerObject.transform.position);

                    MoveAlongPath(rune);

                    WaitingOnPath = false;
                    Pathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
                    FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

                }

                break;

        }
    }

    #endregion WIND FUNCTIONS



    #region KNOCKBACK FUNCTIONS

    //is this messed up or what
    /// <summary>
    /// checks whether or not an enemy can be knocked backwards
    /// </summary>
    /// <param name="kbSource"> where the target is being pushed from </param>
    /// <param name="kbTarget"> the target tile </param>
    /// <returns> whether or not the enemy can be knocked backwards </returns>
    public static bool CanMoveBackwards(TileBehaviour kbSource, TileBehaviour kbTarget)
    {

        Vector2Int newTilePos = kbTarget.IndexInGrid;

        //finds where the target should go
        if (kbSource.IndexInGrid.x < kbTarget.IndexInGrid.x)
        {
            newTilePos.x += 1;
        }
        else if (kbSource.IndexInGrid.x > kbTarget.IndexInGrid.x)
        {
            newTilePos.x -= 1;
        }

        if (kbSource.IndexInGrid.y < kbTarget.IndexInGrid.y)
        {
            newTilePos.y += 1;
        }
        else if (kbSource.IndexInGrid.y > kbTarget.IndexInGrid.y)
        {
            newTilePos.y -= 1;
        }

        TileBehaviour newTile = null;

        //checks if the tile exists
        foreach (TileBehaviour viableTile in GridManager.combatGrid)
        {

            if (viableTile.IndexInGrid == newTilePos)
            {
                newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];
                break;
            }

        }

        //skipped if the tile is not in the grid
        if (newTile != null)
        {

            //putting the or statement here as a bit of extra security even if it's unnecessary while i'm looking for a fix
            if (newTile.GetComponentInChildren<Enemy>() || newTile.entityOnGrid == -2)
            {

                //if there's an enemy on the target tile, this checks if it can be moved backwards as well
                //loops, ideally
                if (CanMoveBackwards(kbTarget, newTile))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return newTile.entityOnGrid == -1 || newTile.entityOnGrid == -20 || newTile.entityOnGrid == -8;
            }

        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// shoves the enemy backwards relative from where wind 1 was initially cast
    /// </summary>
    /// <param name="kbSource"> tile that the player occupies </param>
    /// <param name="kbTarget"> tile that the enemy occupies </param>
    /// <param name="target"> the target </param>
    void SendEnemyBackwards(TileBehaviour kbSource, TileBehaviour kbTarget, Enemy target)
    {

        WindCurrentTracker[] trackers = FindObjectsByType<WindCurrentTracker>(FindObjectsSortMode.None);

        Vector2Int newTilePos = kbTarget.IndexInGrid;

        if (kbSource.IndexInGrid.x < kbTarget.IndexInGrid.x)
        {
            newTilePos.x += 1;
        }
        else if (kbSource.IndexInGrid.x > kbTarget.IndexInGrid.x)
        {
            newTilePos.x -= 1;
        }

        if (kbSource.IndexInGrid.y < kbTarget.IndexInGrid.y)
        {
            newTilePos.y += 1;
        }
        else if (kbSource.IndexInGrid.y > kbTarget.IndexInGrid.y)
        {
            newTilePos.y -= 1;
        }

        TileBehaviour newTile = null;

        foreach (TileBehaviour viableTile in GridManager.combatGrid)
        {

            if (viableTile.IndexInGrid == newTilePos)
            {

                newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

                if (newTile.entityOnGrid == -2)
                {

                    newTile.GetComponentInChildren<Enemy>().Damage(currentSecondaryDamage, Enemy.DamageType.Wind);

                    SendEnemyBackwards(kbTarget, newTile, newTile.GetComponentInChildren<Enemy>());

                    target.transform.SetParent(newTile.transform);

                    target.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                    GridManager.MoveToTile(kbTarget.IndexInGrid, newTilePos, -2);

                    target.GetComponent<GridPathfinding>().SetPosition(newTilePos);
                    FindFirstObjectByType<PlayerBehavior>().UpdateEnemyPositions();

                }
                else
                {

                    target.transform.SetParent(newTile.transform);

                    target.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                    GridManager.MoveToTile(kbTarget.IndexInGrid, newTilePos, -2);

                    target.GetComponent<GridPathfinding>().SetPosition(newTilePos);
                    FindFirstObjectByType<PlayerBehavior>().UpdateEnemyPositions();

                    foreach (WindCurrentTracker tracker in trackers)
                    {

                        if (tracker.WindCurrentTiles.Contains(newTile))
                        {

                            target.Damage(tracker.CurrentDamage, Enemy.DamageType.Wind);
                            tracker.SendThroughWindCurrent(tracker.WindCurrentTiles.IndexOf(newTile), target);

                        }

                    }

                }

                    break;
            }
        }
    }

    /// <summary>
    /// pulls enemy towards a tile
    /// </summary>
    /// <param name="originTile"> the tile being pulled towards </param>
    /// <param name="enemyTile"> the tile that the enemy is originally on</param>
    /// <param name="enemy"> the enemy being pulled towards another tile </param>
    void PullEnemyForward(TileBehaviour originTile, TileBehaviour enemyTile, Enemy enemy)
    {

        Vector2Int newTilePos = enemyTile.IndexInGrid;

        if (originTile.IndexInGrid.x < enemyTile.IndexInGrid.x)
        {

            newTilePos.x -= 2;

        }
        else if (originTile.IndexInGrid.x > enemyTile.IndexInGrid.x)
        {

            newTilePos.x += 2;

        }

        if (originTile.IndexInGrid.y < enemyTile.IndexInGrid.y)
        {

            newTilePos.y -= 2;

        }
        else if (originTile.IndexInGrid.y > enemyTile.IndexInGrid.y)
        {

            newTilePos.y += 2;

        }

        if (GridManager.combatGrid[newTilePos.x, newTilePos.y])
        {

            TileBehaviour newTile = GridManager.combatGrid[newTilePos.x, newTilePos.y];

            if (newTile.entityOnGrid == -1 && newTile != originTile)
            {

                enemy.transform.SetParent(newTile.transform);

                enemy.transform.position = new Vector3(newTile.transform.position.x, 0, newTile.transform.position.z);

                GridManager.MoveToTile(enemyTile.IndexInGrid, newTilePos, -2);

                enemy.GetComponent<GridPathfinding>().SetPosition(newTilePos);
                FindFirstObjectByType<PlayerBehavior>().UpdateEnemyPositions();

            }

        }

    }

    #endregion KNOCKBACK FUNCTIONS



    #region PATHING

    /// <summary>
    /// checks to see if a tile can be pathed through
    /// </summary>
    /// <param name="tileCoordinates"> the tile that the player is attempting to highlight </param>
    /// <returns> status of a tile </returns>
    bool CanMoveThroughTile(Vector2Int tileCoordinates)
    {

        if(selectedRune.TypeOfRune == RuneType.Wind && selectedRune.NumberOnSkillTree == 3)
        {
            return GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -2 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -3 ||
            GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -20;
        }
        
        return GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -1 ||
        GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -2 ||
        GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -3 ||
        GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -8 ||
        GridManager.combatGrid[tileCoordinates.x, tileCoordinates.y].entityOnGrid == -20;

    }

    /// <summary>
    /// determines where the player is attempting to path
    /// </summary>
    /// <param name="dir"> the direction that the player moves in </param>
    private void MoveDirection(Vector2 dir)
    {

        StopCoroutine("ConfirmationDelay");
        if(GetComponent<RuneRangeAndTargeting>().Confirm != null)
        {
            GetComponent<RuneRangeAndTargeting>().Confirm.interactable = false;
        }
        
        if (WaitingOnPath)
        {

            if (dir.y >= .5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x, selectedTile.y + 1);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z + GridManager.MoveDistances.y);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.y <= -.5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x, selectedTile.y - 1);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x, ghostPos.y, ghostPos.z - GridManager.MoveDistances.y);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x > .5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x + 1, selectedTile.y);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x + GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdateMovement(v, newPosition);
                }
            }
            else if (dir.x < -.5f)
            {
                Vector2Int v = new Vector2Int(selectedTile.x - 1, selectedTile.y);

                if (GridManager.TileIsInGrid(v) && CanMoveThroughTile(v))
                {
                    Vector3 newPosition = new Vector3(ghostPos.x - GridManager.MoveDistances.x, ghostPos.y, ghostPos.z);
                    UpdateMovement(v, newPosition);
                }
            }

        }

        StartCoroutine("ConfirmationDelay");

    }

    //used specifically for wind 1
    bool stoppedByEnemy = false;

    /// <summary>
    /// either adds, removes, or rejects a tile wrt pathing
    /// </summary>
    /// <param name="v"></param>
    /// <param name="t"></param>
    private void UpdateMovement(Vector2Int v, Vector3 t)
    {

        StopCoroutine("MovementDelay");
        WaitingOnPath = false;
  
        if (PreviousPos.Contains(v))
        {

            if(PreviousPos.Count > 1)
            {

                ghostPos = new Vector3(PreviousPos[PreviousPos.Count - 2].x, 0, PreviousPos[PreviousPos.Count - 2].y);
                selectedTile = PreviousPos[PreviousPos.Count - 2];

                movementPos.Remove(movementPos[movementPos.Count - 1]);

                GetComponent<RuneRangeAndTargeting>().EditViableTiles
               (false, GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y]);

                GridManager.combatGrid[PreviousPos[PreviousPos.Count - 1].x, PreviousPos[PreviousPos.Count - 1].y].ShowHighlight(false);
                PreviousPos.Remove(PreviousPos[PreviousPos.Count - 1]);

                movementLeft++;
                movementUsed--;

                if(stoppedByEnemy)
                {

                    stoppedByEnemy = false;

                }

            }

        }
        else
        {
            if (movementLeft > 0 && !stoppedByEnemy)
            {

                switch(selectedRune.TypeOfRune, selectedRune.NumberOnSkillTree)
                {

                    case (RuneType.Lightning, 4):

                        if(movementLeft > 1)
                        {

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().LightningSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                            PreviousPos.Add(v);
                            movementPos.Add(t);

                            GetComponent<RuneRangeAndTargeting>().EditViableTiles(true, GridManager.combatGrid[v.x, v.y]);

                            --movementLeft;
                            ++movementUsed;

                            selectedTile = v;

                            ghostPos = t;

                        }
                        else if (movementLeft == 1 && !GridManager.combatGrid[v.x, v.y].GetComponentInChildren<Enemy>())
                        {

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                            PreviousPos.Add(v);
                            movementPos.Add(t);

                            GetComponent<RuneRangeAndTargeting>().EditViableTiles(true, GridManager.combatGrid[v.x, v.y]);

                            --movementLeft;
                            ++movementUsed;

                            selectedTile = v;

                            ghostPos = t;

                        }

                            break;


                    case (RuneType.Wind, 1):

                        if((GridManager.playerPosition.x < originalSelectedTile.x && originalSelectedTile.x < v.x) ||
                        (GridManager.playerPosition.x > selectedTile.x && originalSelectedTile.x > v.x) ||
                        (GridManager.playerPosition.y < selectedTile.y && originalSelectedTile.y < v.y) ||
                        (GridManager.playerPosition.y > selectedTile.y && originalSelectedTile.y > v.y))
                        {

                            if (GridManager.combatGrid[v.x, v.y].GetComponentInChildren<PlayerBehavior>())
                            {

                                StartCoroutine(MovementDelay());

                                return;

                            }

                            if(GridManager.combatGrid[v.x, v.y].GetComponentInChildren<Enemy>())
                            {

                                if (!CanMoveBackwards(GridManager.combatGrid[selectedTile.x, selectedTile.y], GridManager.combatGrid[v.x, v.y]))
                                {

                                    StartCoroutine(MovementDelay());

                                    return;

                                }

                                stoppedByEnemy = true;

                            }

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                            PreviousPos.Add(v);
                            movementPos.Add(t);

                            GetComponent<RuneRangeAndTargeting>().EditViableTiles(true, GridManager.combatGrid[v.x, v.y]);

                            --movementLeft;
                            ++movementUsed;

                            selectedTile = v;

                            ghostPos = t;

                            

                        }

                        break;

                    case (RuneType.Wind, 2):

                        if (GridManager.combatGrid[v.x, v.y].entityOnGrid == -1)
                        {

                            GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                            GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                            PreviousPos.Add(v);
                            movementPos.Add(t);

                            GetComponent<RuneRangeAndTargeting>().EditViableTiles(true, GridManager.combatGrid[v.x, v.y]);

                            --movementLeft;
                            ++movementUsed;

                            selectedTile = v;

                            ghostPos = t;

                        }

                        break;

                    default:

                        if (GridManager.combatGrid[v.x, v.y].GetComponentInChildren<PlayerBehavior>())
                        {

                            StartCoroutine(MovementDelay());

                            return;

                        }

                        GridManager.combatGrid[v.x, v.y].SetHighlightColor(GetComponent<RuneRangeAndTargeting>().WindSecondaryHighlight);
                        GridManager.combatGrid[v.x, v.y].ShowHighlight(true);
                        PreviousPos.Add(v);
                        movementPos.Add(t);

                        GetComponent<RuneRangeAndTargeting>().EditViableTiles(true, GridManager.combatGrid[v.x, v.y]);

                        --movementLeft;
                        ++movementUsed;

                        selectedTile = v;

                        ghostPos = t;

                        break;

                }

            }
        }

        StartCoroutine("MovementDelay");
    }

    /// <summary>
    /// allows the player to select tiles again after a brief pause
    /// </summary>
    /// <returns> waitingonpath </returns>
    IEnumerator MovementDelay()
    {
        yield return new WaitForSeconds(.1f);
        WaitingOnPath = true;
    }

    IEnumerator ConfirmationDelay()
    {
        yield return new WaitForSeconds(.2f);
        if (GetComponent<RuneRangeAndTargeting>().Confirm != null)
        {
            GetComponent<RuneRangeAndTargeting>().Confirm.interactable = true;
        }
    }

    //used for certain wind attacks
    float currentSecondaryDamage;

    List<Enemy> targetedEnemies = new List<Enemy>();

    /// <summary>
    /// executes attacks that utilize pathing
    /// </summary>
    /// <param name="rune"> selected rune </param>
    void MoveAlongPath(RuneData rune)
    {

        WindCurrentTracker[] trackers = FindObjectsByType<WindCurrentTracker>(FindObjectsSortMode.None);

        float damageDealt = 0;

        if(rune.TypeOfRune == RuneType.Wind)
        {

            damageDealt = Mathf.Ceil(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

            currentSecondaryDamage = Mathf.Ceil(rune.SecondaryRuneDamage * FindFirstObjectByType<PlayerStats>().WindAttackMultiplier *
            FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        }
        else if (rune.TypeOfRune == RuneType.Lightning)
        {

            damageDealt = Mathf.Ceil(rune.RuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier
            * FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

            currentSecondaryDamage = Mathf.Ceil(rune.SecondaryRuneDamage * FindFirstObjectByType<PlayerStats>().LightningAttackMultiplier *
            FindFirstObjectByType<PlayerStats>().BaseAttackMultiplier);

        }

        switch (rune.TypeOfRune, rune.NumberOnSkillTree)
        {

            case (RuneType.Lightning, 4):

                FindFirstObjectByType<PlayerBehavior>().gameObject.transform.SetParent
                (GridManager.combatGrid[selectedTile.x, selectedTile.y].transform);

                FindFirstObjectByType<PlayerBehavior>().gameObject.transform.position = new Vector3
                (GridManager.combatGrid[selectedTile.x, selectedTile.y].transform.position.x, 0, 
                GridManager.combatGrid[selectedTile.x, selectedTile.y].transform.position.z);

                GridManager.MoveToTile(originalSelectedTile, selectedTile, -3);

                Invoke("PlayerTeleport", .2f);

                AudioManager.instance.CreateEventInstance(lightningSpellSFX_4);
                AudioManager.instance.PlayOneShot(lightningSpellSFX_4, audioListenerObject.transform.position);

                for(int i = 0; i < PreviousPos.Count; i++)
                {

                    Instantiate(rune.RuneVFX, GridManager.combatGrid[selectedTile.x, selectedTile.y].transform);

                    if (GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>())
                    {

                        GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>().Damage
                        (damageDealt, Enemy.DamageType.Lightning);

                    }

                    GridManager.combatGrid[selectedTile.x, selectedTile.y].Invoke("ElectrifyAdTiles", 1.2f);

                }

                break;

            case (RuneType.Wind, 1):

                for (int i = 0; i < movementPos.Count; ++i)
                {

                    Vector2Int nextPos = PreviousPos[i + 1];

                    if(i == 0)
                    {

                        Instantiate(rune.RuneVFX, GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform);

                    }

                    if (GridManager.combatGrid[nextPos.x, nextPos.y].GetComponentInChildren<Enemy>())
                    {

                        GridManager.combatGrid[nextPos.x, nextPos.y].GetComponentInChildren<Enemy>().Damage(currentSecondaryDamage, Enemy.DamageType.Wind);

                        if(CanMoveBackwards(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y], GridManager.combatGrid[nextPos.x, nextPos.y]))
                        {

                            SendEnemyBackwards(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y],
                            GridManager.combatGrid[nextPos.x, nextPos.y],
                            GridManager.combatGrid[nextPos.x, nextPos.y].GetComponentInChildren<Enemy>());

                        }

                    }

                    if(i == (movementPos.Count - 1))
                    {

                        foreach (WindCurrentTracker tracker in trackers)
                        {

                            if (tracker.WindCurrentTiles.Contains(GridManager.combatGrid[nextPos.x, nextPos.y]))
                            {

                                selectedEnemy.Damage(tracker.CurrentDamage, Enemy.DamageType.Wind);

                                tracker.SendThroughWindCurrent(tracker.WindCurrentTiles.IndexOf
                                (GridManager.combatGrid[nextPos.x, nextPos.y]), selectedEnemy);

                                PreviousPos.Clear();
                                movementPos.Clear();
                                movementUsed = 0;

                                StartCoroutine(UpdatePlayerStatus());

                                return;

                            }

                        }

                        selectedEnemy.transform.SetParent(GridManager.combatGrid[nextPos.x, nextPos.y].transform);

                        selectedEnemy.transform.position = new Vector3(GridManager.combatGrid[nextPos.x, nextPos.y].transform.position.x,
                        0, GridManager.combatGrid[nextPos.x, nextPos.y].transform.position.z);

                        GridManager.MoveToTile(PreviousPos[i], nextPos, -2);

                        selectedEnemy.GetComponent<GridPathfinding>().SetPosition(nextPos);
                        FindFirstObjectByType<PlayerBehavior>().UpdateEnemyPositions();

                    }

                }

                break;

            case (RuneType.Wind, 2):

                for(int i = 0; i < movementPos.Count; ++i)
                {

                    ShieldBehavior newShield = GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].gameObject.AddComponent<ShieldBehavior>();
                    newShield.OnShieldGenerated(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform, rune.RuneVFX);

                    GridManager.AddEntity(PreviousPos[i], -7);

                }

                break;

            case (RuneType.Wind, 3):

                WindCurrentTracker currentTracker = FindFirstObjectByType<GameManager>().gameObject.AddComponent<WindCurrentTracker>();

                currentTracker.CurrentDamage = damageDealt;
                currentTracker.CurrentKBDamage = currentSecondaryDamage;

                for(int i = 0; i < movementPos.Count; i++)
                {

                    currentTracker.WindCurrentTiles.Add(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y]);
                    GridManager.AddEntity(PreviousPos[i], -8);

                    if(i == movementPos.Count - 1)
                    {
                        currentTracker.GenerateWindCurrent(rune.RuneVFX);
                    }

                    if (GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>())
                    {
                        GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Wind);
                        currentTracker.SendThroughWindCurrent(i, GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>());
                    }

                }

                break;

            case (RuneType.Wind, 4):

                targetedEnemies.Clear();

                foreach(Vector2Int tile in PreviousPos)
                {

                    if (GridManager.combatGrid[tile.x, tile.y].GetComponentInChildren<Enemy>())
                    {

                        GridManager.combatGrid[tile.x, tile.y].GetComponentInChildren<Enemy>().Damage(damageDealt, Enemy.DamageType.Wind);

                        targetedEnemies.Add(GridManager.combatGrid[tile.x, tile.y].GetComponentInChildren<Enemy>());

                    }

                }

                for(int i = 0; i < movementPos.Count; i++)
                {

                    Instantiate(rune.RuneVFX, GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].transform);

                    if (GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>())
                    {

                        if(i != 0 && CanMoveBackwards(GridManager.combatGrid[PreviousPos[i - 1].x, PreviousPos[i - 1].y],
                        GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y]))
                        {

                            SendEnemyBackwards(GridManager.combatGrid[PreviousPos[i - 1].x, PreviousPos[i - 1].y],
                            GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y],
                            GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y].GetComponentInChildren<Enemy>());

                        }

                    }

                    if(i == movementPos.Count - 1)
                    {

                        PublicEvents.CheckRange.Invoke(true, rune.RuneAOE, GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y]);

                        foreach (TileBehaviour tileInRange in targetedTiles)
                        {


                            if (tileInRange.GetComponentInChildren<Enemy>() != null)
                            {

                                if(!targetedEnemies.Contains(tileInRange.GetComponentInChildren<Enemy>()))
                                {

                                    tileInRange.GetComponentInChildren<Enemy>().Damage(currentSecondaryDamage, Enemy.DamageType.Wind);

                                    PullEnemyForward(GridManager.combatGrid[PreviousPos[i].x, PreviousPos[i].y], tileInRange,
                                    tileInRange.GetComponentInChildren<Enemy>());

                                }

                            }

                        }

                    }

                }

                break;

        }


        PreviousPos.Clear();
        movementPos.Clear();
        movementUsed = 0;

        StartCoroutine(UpdatePlayerStatus());

    }

    /// <summary>
    /// clears pathing if the player cancels their attack
    /// </summary>
    public void CancelPathing()
    {

        WaitingOnPath = false;
        Pathing = false;

        FindFirstObjectByType<PlayerInputHandler>().IsPathing = false;
        FindFirstObjectByType<PlayerInputHandler>().enableMovement = false;

        PreviousPos.Clear();
        movementPos.Clear();
        movementLeft += movementUsed;
        movementUsed = 0;

    }

    #endregion PATHING



    #region END TURN

    /// <summary>
    /// ends the player's turn a second after they attack
    /// the timing can be made into a variable later mb
    /// </summary>
    /// <returns> one second </returns>
    IEnumerator UpdatePlayerStatus()
    {

        int timer = 0;

        while (timer <= 4)
        {

            timer++;

            if (timer == 4)
            {

                PublicEvents.EndCast.Invoke();
                Casting = false;
                anim.SetBool("Attack", false);
                bookanim.SetBool("LAtk", false);
                bookanim.SetBool("WAtk", false);
                bookanim.SetBool("Idle", true);
                anim.SetBool("Idle", true);
            }

            yield return new WaitForSeconds(1);

        }

    }

    /// <summary>
    /// Used to create a delay between teleporting the player and updating the variables
    /// </summary>
    private void PlayerTeleport()
    {
        Debug.Log("ughhhhhhhhhhhhhhhhhhh"); //The code-bearing debug.log. I'm not kidding, this stops an error from happening
        FindFirstObjectByType<PlayerBehavior>().TeleportPlayer();
    }

    #endregion END TURN

}
