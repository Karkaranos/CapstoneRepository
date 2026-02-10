/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/28/2025
Date Last Modified : 11/24/2025
Brief Description : This manages the individual nodes
                    on the skill tree.
External Resources : 	
	***************************************************/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeNode : MonoBehaviour
{
    #region VARS

    //this is the inspector enum - only controls what the inspector sees
    private enum Settings
    {
        NodeSettings,
        SkillSettings,
        Refrences
    }

    //this is the enum for what status the node is and determines how the player
    //can interact with the nodes
    public enum  NodeStatus
    {
        Locked,
        Unlocked,
        Purchased
    }

    //what setting the inspector is currently in
    [SerializeField, Tooltip("This changes what settings are shown in the inspector")] private Settings currentSettings;

    #region NODE SETTINGS

    [HorizontalLine(4, EColor.Red)]

    //how much the node costs to purchase
    [SerializeField, ShowIf(nameof(currentSettings), Settings.NodeSettings), Tooltip("How much the node costs to purchase")]
    private int cost;

    //the list of nodes that are required before this node unlocks. 
    [ShowIf(nameof(currentSettings), Settings.NodeSettings), SerializeField, Tooltip("This is the list of nodes that are required before" +
        " this node unlocks. If its empty, it will be unlocked when the skill tree is initialized.")]
    private List<SkillTreeNode> PrereqNodes;

    //This toggles whether this node requires all prereqs before unlocking or just one prereq before unlocking.
    [ShowIf(nameof(currentSettings), Settings.NodeSettings), SerializeField, Tooltip("This toggles whether this node requires all prereqs" +
        " before unlocking or just one prereq before unlocking.")]
    private bool RequireAllPrereqsBeforeUnlocking;

    //This is the list of nodes that become permanantly locked when this node is purchased.
    [ShowIf(nameof(currentSettings), Settings.NodeSettings), SerializeField, Tooltip("This is the list of nodes that" +
        " become permanantly locked when this node is purchased.")]
    private List<SkillTreeNode> OppositeNodes;

    [ShowIf(nameof(currentSettings), Settings.NodeSettings), SerializeField] private bool IsInSkillTree;

    [ShowIf(nameof(currentSettings), Settings.NodeSettings), SerializeField] private bool StartUnlocked;

    #endregion

    #region SKILL SETTINGS

    [HorizontalLine(4, EColor.Indigo)]
    //Holds the rune that gets unlocked when the node is purchased
    [ShowIf(nameof(currentSettings), Settings.SkillSettings), Expandable,
        Tooltip("This is the rune that the player unlocks when purchasing this node")]
    public RuneData NodeRuneData;

    #endregion


    #region NONINSPECTOR VARS

    private NodeStatus status;

    //This node's current status
    public NodeStatus Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    //The button component on the same object as this script
    private Button button;

    //Determines whether or not this node is permanantly locked. Only used for opposite nodes
    private bool isPermaLocked;

    //A reference to the skill tree manager in the scene
    [SerializeField, ShowIf(nameof(currentSettings), Settings.Refrences)] private SkillTreeManager skillTreeManager;

    //A Reference to the artifactandskilltree manager in the scene
    private SkillAndArtifactManager skillAndArtifactManager;

    #endregion
    #endregion

    #region FUNCS

    #region INITIALIZATION

    /// <summary>
    /// Sets refs
    /// </summary>
    private void Awake()
    {
        //skillTreeManager = FindFirstObjectByType<SkillTreeManager>();
        skillAndArtifactManager = FindFirstObjectByType<SkillAndArtifactManager>();
        button = GetComponent<Button>();
        isPermaLocked = false;

        if (StartUnlocked)
        {
            Status = NodeStatus.Purchased;
            //button.interactable = false;
            GetComponent<Image>().color = Color.green;
            //Debug.Log("Node Purchased");

            //updates the skill tree manager with the rune that got purchased
            skillTreeManager.UpdatePurchasedNodes(NodeRuneData);
        }
    }

    /// <summary>
    /// Subscribes to all public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.SkillTreeNodePurchased += AnySkillTreeNodePurchased;
        PublicEvents.TrashHeldOOCObject += TurnNodeBackOnMaybe;

        if (!IsInSkillTree && status != NodeStatus.Purchased)
        {
            UpdateNodeStatus();
        }
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.SkillTreeNodePurchased -= AnySkillTreeNodePurchased;
        PublicEvents.TrashHeldOOCObject -= TurnNodeBackOnMaybe;
    }

    /// <summary>
    /// Initializes in start not awake to make sure everything else in the scene is initialized.
    /// </summary>
    private void Start()
    {
        //finds the refs in scene
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();
        skillAndArtifactManager = FindFirstObjectByType<SkillAndArtifactManager>();

        if (IsInSkillTree)
        {
            //starts locked/unlocked depending on if it has any prereqs
            if (PrereqNodes.Count > 0)
            {
                LockNode();
            }
            else
            {
                UnlockNode();
            }
        }


    }

    #endregion

    #region NODE STATUS FUNCS

    /// <summary>
    /// Updates the status of the nodes in the character menu
    /// </summary>
    private void UpdateNodeStatus()
    {
        if (skillAndArtifactManager.UnlockedRunes.Contains(NodeRuneData))
        {
            if (!skillAndArtifactManager.equippedSpells.Contains(NodeRuneData))
            {
                Status = NodeStatus.Purchased;
                button.interactable = true;
                GetComponent<Image>().color = Color.green;
            }
            
        }
        else
        {
            LockNode();
        }
    }

    /// <summary>
    /// This locks the node, making it uninteractable for now
    /// </summary>
    private void LockNode()
    {
        Status = NodeStatus.Locked;
        button.interactable = false;
        if (IsInSkillTree)
        {
            GetComponent<Image>().color = Color.gray;
        }
        
        //Debug.Log("Node Locked");
    }

    /// <summary>
    /// Unlocks the node, making it available for purchase
    /// </summary>
    private void UnlockNode()
    {
        Status = NodeStatus.Unlocked;
        button.interactable = true;
        GetComponent<Image>().color = Color.white;
        //Debug.Log("Node Unlocked");
    }

    /// <summary>
    /// This handles the logic for purchasing the node
    /// </summary>
    public void PurchaseNode()
    {
        if (!skillAndArtifactManager.UnlockedRunes.Contains(NodeRuneData))
        {
            //Checks with the skill tree manager to see if it can be purchased
            if (skillTreeManager.CanPurchaseNode(cost))
            {
                Status = NodeStatus.Purchased;
                //button.interactable = false;
                GetComponent<Image>().color = Color.green;
                //Debug.Log("Node Purchased");

                //updates the skill tree manager with the rune that got purchased
                skillTreeManager.UpdatePurchasedNodes(NodeRuneData);

                //updates the description with this node, but shows that you own it
                //instead of showing the cost
                //skillTreeManager.UpdateSpellDescriptionText(NodeRuneData, -1);

                //If it has any opposite nodes, it permanantly locks them
                if (OppositeNodes.Count > 0)
                {
                    foreach (SkillTreeNode node in OppositeNodes)
                    {
                        node.PermanantlyLockNode();
                    }
                }

                //Tells all other nodes that a node was purchased
                PublicEvents.SkillTreeNodePurchased();
            }
            else
            {
                //Debug.Log("Too Few SkillPoints to Purchase Node");
            }
        }
        

    }

    /// <summary>
    /// This permanantly locks the node.
    /// </summary>
    public void PermanantlyLockNode()
    {
        LockNode();
        //gameObject.SetActive(false);
        button.interactable = false;
        isPermaLocked = true;
    }

    /// <summary>
    /// This triggers when the public event SkillTreeNodePurchased() is called.
    /// Unlocks the node if it can be unlocked
    /// </summary>
    private void AnySkillTreeNodePurchased()
    {
        //checks to see if its currently locked and is not permalocked
        if (status == NodeStatus.Locked && !isPermaLocked && IsInSkillTree)
        {
            //checks the prereqs
            foreach (SkillTreeNode node in PrereqNodes)
            {
                //if all prereqs are required, if any prereqs aren't purchased it quits this and stays locked
                if (node.status != NodeStatus.Purchased && RequireAllPrereqsBeforeUnlocking)
                {
                    return;
                }

                //if only one prereq is required, unlocks when one prereq is purchased
                if (node.status == NodeStatus.Purchased && !RequireAllPrereqsBeforeUnlocking)
                {
                    UnlockNode();
                }
            }

            //if it got to this point, all of the prereqs are purchased so it unlocks the node.
            if (RequireAllPrereqsBeforeUnlocking)
            {
                UnlockNode();
            }
        }
    }

    /// <summary>
    /// Picks up the spell so you can equip it when you 
    /// click on this node while you own it
    /// </summary>
    public void SelectNodeWhilePurchased()
    {
        skillTreeManager.SelectNode(NodeRuneData);
        button.interactable = false;
    }


    /// <summary>
    /// This is triggered when the button is clicked on.
    /// </summary>
    public void ClickedOn()
    {
        //trys to buy node if node is unlocked.
        switch (status)
        {
            case NodeStatus.Locked:
                Debug.Log("Clicked on a locked node - check logic to make sure button is uninteractable here");
                break;
            case NodeStatus.Unlocked:
                if (IsInSkillTree)
                {
                    OnHover();
                }
                // PurchaseNode();
                break;
            case NodeStatus.Purchased:
                if (IsInSkillTree)
                {
                    OnHover();
                }
                else
                {
                    SelectNodeWhilePurchased();
                }                    
                break;
            default:
                Debug.Log("Tyler update the fucking switch statement in clickedon() in skilltreenode - missing cases");
                break;
        }
    }

    /// <summary>
    /// triggers when the button is hovered over
    /// tells the description to update with the data in this node
    /// </summary>
    public void OnHover()
    {
        //updates the text showing you own it if you own the node
        if (status == NodeStatus.Purchased)
        {
            skillTreeManager.UpdateSpellDescriptionText(this, -1);
        }
        else
        {
            skillTreeManager.UpdateSpellDescriptionText(this, cost);
        }
    }

    /// <summary>
    /// Re-enables the node if the player doesn't have it equipped
    /// </summary>
    private void TurnNodeBackOnMaybe()
    {
        if (!skillAndArtifactManager.equippedSpells.Contains(NodeRuneData))
        {
            button.interactable = true;
        }
    }
    #endregion

    #endregion
}
