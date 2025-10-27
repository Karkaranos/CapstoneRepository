/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    10/5/2025
Date Last Modified : 	10/26/2025
Brief Description : 	Controls what artifacts and effects are actively applied                     
External Resources : 	https://stackoverflow.com/questions/1420186/references-to-variables-in-c
***************************************************/
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArtifactManager
{
    #region Variables
    [SerializeField] private bool inTestMode;

    #region Artifacts
    // Stores all currently applied Artifacts
    private static List<ArtifactData> currentArtifacts = new List<ArtifactData>();
    private static int currentArtifactWeight = 0;

    // Stores all Artifacts not currently in use
    private static List<ArtifactData> inventoryArtifacts = new List<ArtifactData>();

    [SerializeField, Tooltip("How many Artifacts can be applied at once")] private static int maxArtifactWeight = 3;


    private static ArtifactData[] testData;
    private static ArtifactData[] randomArtifactPool;
    private static ArtifactData[] setArtifactPool;

    public static int MaxArtifactWeight { get => maxArtifactWeight; set => maxArtifactWeight = value; }

    #endregion Artifacts

    #region Stamps
    private static Dictionary<MarkType, int> markCount = new Dictionary<MarkType, int>();
    private static List<ArtifactData> triggerOnAttack = new List<ArtifactData>();

    #endregion Stamps

    private static PlayerStats player;
    private static GameManager gameManager;

    #endregion Variables

    #region Functions

    #region Initialization
    /// <summary>
    /// Constructor for Artifact Manager
    /// </summary>
    /// <param name="rap">Random Artifact Pool</param>
    /// <param name="sap">Set Artifact Pool</param>
    /// <param name="maxArtifact">Maximum Number of Artifacts</param>
    /// <param name="testing">True if testing functionality</param>
    /// <param name="testInfo">Data for testing. Please have a minimum length of 4</param>
    public ArtifactManager(ArtifactData[] rap, ArtifactData[] sap, int maxArtifact, PlayerStats p, GameManager gm, bool testing = false, ArtifactData[] testInfo = null)
    {
        randomArtifactPool = rap;
        setArtifactPool = sap;
        maxArtifactWeight = maxArtifact;
        player = p;
        gameManager = gm;

        // Create an entry in the dictionary for each mark type
        for(int i=0; i<MarkType.GetNames(typeof(MarkType)).Length; i++)
        {
            markCount.Add((MarkType)i, 0);
        }

        if (testing)
        {
            inTestMode = true;
            testData = testInfo;
            TestArtifacts();
        }

    }

    /// <summary>
    /// Creates listeners
    /// </summary>
    void OnEnable()
    {
        PublicEvents.RuneCast += PlayerAttack;
    }

    /// <summary>
    /// Unassigns listeners
    /// </summary>
    void OnDisable()
    {
        PublicEvents.RuneCast -= PlayerAttack;
    }

    /// <summary>
    /// Sets a reference to the PlayerStats script
    /// </summary>
    /// <param name="p">PlayerStats</param>
    /// <returns>Whether the operaation was sucessful or not</returns>
    public static void SetPlayerReference(PlayerStats p)
    {
        player = p;
    }

    /// <summary>
    /// Hardcoded function to show adding/removing
    /// Applies the first three items, then tries to apply a fourth
    /// Removes an item then applies the fourth
    public static void TestArtifacts()
    {
        ApplyArtifact(testData[0]);
        if (testData[0].TriggerCondition == ArtifactTriggerCondition.OnAttack)
        {
            PlayerAttack(1);
        }
    }

    #endregion

    #region Get From Pools

    /// <summary>
    /// Gets an artifact from the Set Artifact Pool
    /// Returns an artifact based on current level
    /// </summary>
    /// <param name="level">Current level, 0-level count-1</param>
    /// <returns>related ArtifactData</returns>
    /// <exception cref="System.Exception">Throws out of index exception</exception>
    public static ArtifactData GetArtifactFromSAP(int level)
    {
        if (level <= setArtifactPool.Length)
        {
            return setArtifactPool[level];
        }
        throw new System.Exception("Cannot access indexes outside of the SAP Array");
    }

    /// <summary>
    /// Returns an artifact from the RandomArtifactPool
    /// </summary>
    /// <returns>Random ArtifactData</returns>
    public static ArtifactData GetArtifactFromRAP()
    {
        return randomArtifactPool[UnityEngine.Random.Range(0, randomArtifactPool.Length)];
    }

    #endregion

    #region Adding and Removing
    /// <summary>
    /// Adds the ArtifactData to the player's pool
    /// </summary>
    /// <param name="artifact">ArtifactData to add</param>
    public static void ObtainArtifact(ArtifactData artifact)
    {
        inventoryArtifacts.Add(artifact);
        Logger.Log("Added " + artifact.Name + " to inventory");
    }

    /// <summary>
    /// Equips an Artifact and updates the stats accordingly
    /// </summary>
    /// <param name="artifact">The artifact to add</param>
    public static void ApplyArtifact(ArtifactData artifact)
    {
        if (currentArtifactWeight + artifact.ArtifactSize <= MaxArtifactWeight)
        {
            currentArtifacts.Add(artifact);
            if(artifact.TriggerCondition == ArtifactTriggerCondition.OnEquip)
            {
                TriggerOnEquipEffect(artifact, true);
            }
            else if (artifact.TriggerCondition == ArtifactTriggerCondition.OnAttack)
            {
                triggerOnAttack.Add(artifact);
            }
                inventoryArtifacts.Remove(artifact);
            UpdateDictionary(artifact.Mark, true);
            MarkManager.EquipValueChanged(artifact.Mark, markCount[artifact.Mark], true, player);
            currentArtifactWeight += artifact.ArtifactSize;
            
        }
        else
        {
            Logger.Warning("Too many Artifacts applied");
        }
    }

    /// <summary>
    /// Removes an artifact
    /// Reverses its effects
    /// </summary>
    /// <param name="artifact">The artifact to remove</param>
    public static void RemoveArtifact(ArtifactData artifact)
    {
        if (currentArtifacts.Contains(artifact))
        {
            inventoryArtifacts.Add(artifact);
            if (artifact.TriggerCondition == ArtifactTriggerCondition.OnEquip)
            {
                TriggerOnEquipEffect(artifact, false);
            }
            else if (artifact.TriggerCondition == ArtifactTriggerCondition.OnAttack)
            {
                triggerOnAttack.Remove(artifact);
            }
            currentArtifacts.Remove(artifact);
            MarkManager.EquipValueChanged(artifact.Mark, markCount[artifact.Mark], false, player);
            UpdateDictionary(artifact.Mark, false);
            currentArtifactWeight -= artifact.ArtifactSize;
        }
        else
        {
            Logger.Warning("Could not find Artifact");
        }
    }

    #endregion

    #region Effect Triggers
    /// <summary>
    /// When the player attacks, trigger all artifacts with the OnAttack trigger
    /// </summary>
    /// <param name="cost">should be changed to damage dealt later</param>
    private static void PlayerAttack(int cost)
    {
        foreach(ArtifactData ad in triggerOnAttack)
        {
            // Pass damage dealt to this. Defaulted to 10f for now
            TriggerOnAttackEffect(ad);
        }
    }

    /// <summary>
    /// Handles what effects are applied on Equipping them
    /// Pass in true when applying effects, false when removing them
    /// </summary>
    /// <param name="artifact">The artifact to be considered</param>
    /// <param name="adding">Whether the artifact is being applied or not</param>
    private static void TriggerOnEquipEffect(ArtifactData artifact, bool adding = true)
    {
        Debug.Log("Called");

        // Returns if the player does not exist
        if(player==null)
        {
            Debug.LogWarning("Player is null");
            return;
        }

        if(gameManager == null)
        {
            Debug.LogWarning("GameManager is null");
            return;
        }


        string s = artifact.Name + " Effects: ";
        // Adjusts the related effect
        foreach (ArtifactEffects e in artifact.Effects)
        {
            // Some artifacts have a chance to be triggered
            // If the chance is not met, continue to the next effect
            float chance = Random.Range(0f, 1f);
            if (chance > e.TriggerChance)
            {
                continue;
            }


            // The effect should be triggered
            // Trigger the appropriate stat change
            switch (e.Effect)
            {
                case Effects.LightningAttackMultiplier:
                    AdjustValueGeometrically(ref player.LightningAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Lightning Attack multiplied by ";
                    break;
                case Effects.AttackMultiplier:
                    AdjustValueGeometrically(ref player.BaseAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Attack multiplied by ";
                    break;
                case Effects.WindAttackMultiplier:
                    AdjustValueGeometrically(ref player.WindAttackMultiplier, e.StatChangeAmount, adding);
                    s += "Wind Attack multiplied by ";
                    break;
                case Effects.ActionPointChange:
                    AdjustValueArithmetically(ref gameManager.ActionPointsPerTurn, (int)e.StatChangeAmount, adding);
                    s += "Action Points changed by ";
                    break;
                // Special case as their resistance may be 0
                // Add if their resistance is 0, otherwise multiply
                case Effects.ResistanceMultiplier:
                    if(player.Resistance > 0f)
                    {
                        AdjustValueGeometrically(ref player.Resistance, e.StatChangeAmount, adding);
                        s += "Resistance multiplied by ";
                    }
                    else
                    {
                        AdjustValueArithmetically(ref player.Resistance, e.StatChangeAmount, adding);
                        s += "Resistance increased by ";
                    }
                    break;
                case Effects.TotalDamageTakenMultiplier:
                    AdjustValueGeometrically(ref player.DamageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Damage Taken multiplied by ";
                    break;
                // Special case to adjust the player's health (may be cut/unneeded)
                // Saves the current health percent and sets the updated health value to it
                case Effects.HealthChange:
                    float healthPercent = player.CurrentHealth / player.MaxHealth;
                    AdjustValueArithmetically(ref player.MaxHealth, (int)e.StatChangeAmount, adding);
                    player.CurrentHealth = (int)(player.MaxHealth * healthPercent);
                    if(player.CurrentHealth > player.MaxHealth)
                    {
                        player.CurrentHealth = player.MaxHealth;
                    }
                    s += "Health changed by ";
                    break;
                case Effects.SpellSlotsChange:
                    Logger.Warning("Implement Spell Slot Change later");
                    s += "Spell Slot Count changed by ";
                    break;
                /*case Effects.MovementRadiusChange:
                    Logger.Warning("Implement Movement Radius later");
                    s += "Movement Speed multiplied by ";
                    break;*/
                case Effects.RangedDamageTakenMultiplier:
                    AdjustValueGeometrically(ref player.RangedDamageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Ranged Damage Taken multiplied by ";
                    break;
                case Effects.MeleeDamageTakenMultiplier:
                    AdjustValueGeometrically(ref player.MeleeDamageTakenMultiplier, e.StatChangeAmount, adding);
                    s += "Melee Damage Taken multiplied by ";
                    break;
                // Special case as their dodge may be 0
                // Add if their dodge is 0, otherwise multiply
                case Effects.Dodge:
                    if (player.DodgeChance > 0f)
                    {
                        AdjustValueGeometrically(ref player.DodgeChance, e.StatChangeAmount, adding);
                        s += "Dodge multiplied by ";
                    }
                    else
                    {
                        AdjustValueArithmetically(ref player.DodgeChance, e.StatChangeAmount, adding);
                        s += "Dodge increased by ";
                    }
                    break;
                default:
                    break;
            }
            s += e.StatChangeAmount + " | ";
        }

        if (adding)
        {
            Logger.Log(s);
        }
        else
        {
            Logger.Log("Removed " + artifact.Name);
        }

    }

    /// <summary>
    /// Handles what effects are applied on Equipping them
    /// Pass in true when applying effects, false when removing them
    /// </summary>
    /// <param name="artifact">The artifact to be considered</param>
    /// <param name="damageDealt">How much damage the player dealt</param>
    public static void TriggerOnAttackEffect(ArtifactData artifact, float damageDealt = 10f)
    {
        // Adjusts the related effect
        foreach (ArtifactEffects e in artifact.Effects)
        {
            // Some artifacts have a chance to be triggered
            // If the chance is not met, continue to the next effect
            float chance = Random.Range(0f, 1f);
            if (chance > e.TriggerChance)
            {
                continue;
            }

            // The effect should be triggered
            // Trigger the appropriate stat change
            switch (e.Effect)
            {
                case Effects.Vampiric:
                    player.Heal((int)(damageDealt * e.StatChangeAmount));
                    Logger.Log("Healed the player by " + (int)(damageDealt * e.StatChangeAmount) + " due to vampirism");
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// Update the Mark dictionary to reflect the number of marks
    /// </summary>
    /// <param name="key">What mark to find</param>
    /// <param name="adding">Whether the artifact is being added or removed</param>
    private static void UpdateDictionary(MarkType key, bool adding)
    {
        markCount[key] = markCount[key] + (adding ? 1 : -1);
    }

#endregion 

    #region Helper Functions

    /// <summary>
    /// Used for multiplicative or divisive adjustments to values
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value to be multiplied/divided by</param>
    /// <param name="b">True for adding an artifact</param>
    private static void AdjustValueGeometrically(ref float x, float y, bool b)
    {
        x = (b ? x * y : x / y);
    }

    /// <summary>
    /// Used for additive or subtractive adjustments to values using ints
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value being added/subtracted</param>
    /// <param name="b">True for adding an artifact</param>
    private static void AdjustValueArithmetically(ref int x, int y, bool b)
    {
        x = x + (b ? y : y * -1);
    }

    /// <summary>
    /// Used for additive or subtractive adjustments to values using floats
    /// </summary>
    /// <param name="x">The value to be adjusted</param>
    /// <param name="y">The value being added/subtracted</param>
    /// <param name="b">True for adding an artifact</param>
    private static void AdjustValueArithmetically(ref float x, float y, bool b)
    {
        x = x + (b ? y : y * -1);
    }

    #endregion

    #endregion Functions
}
