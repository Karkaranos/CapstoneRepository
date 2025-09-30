/*************************************************
Author Names : 		Tyler Hayes 
Date Created : 		09/28/2025
Date Last Modified : 09/28/2025
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
    public enum NodeStatus
    {
        Locked,
        Unlocked,
        Purchased
    }

    //what setting the inspector is currently in
    [SerializeField, Tooltip("This changes what settings are shown in the inspector")] private Settings currentSettings;

    #region SKILL SETTINGS

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
    private SkillTreeManager skillTreeManager;

    #endregion
    #endregion

    #region FUNCS

    #region INITIALIZATION

    /// <summary>
    /// Sets refs
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
        isPermaLocked = false;
    }

    /// <summary>
    /// Subscribes to all public events
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.SkillTreeNodePurchased += AnySkillTreeNodePurchased;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.SkillTreeNodePurchased -= AnySkillTreeNodePurchased;
    }

    /// <summary>
    /// Initializes in start not awake to make sure everything else in the scene is initialized.
    /// </summary>
    private void Start()
    {
        //finds the skill tree manager in scene
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();

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

    #endregion

    #region NODE STATUS FUNCS

    /// <summary>
    /// This locks the node, making it uninteractable for now
    /// </summary>
    private void LockNode()
    {
        Status = NodeStatus.Locked;
        button.interactable = false;
        //Debug.Log("Node Locked");
    }

    /// <summary>
    /// Unlocks the node, making it available for purchase
    /// </summary>
    public void UnlockNode()
    {
        Status = NodeStatus.Unlocked;
        button.interactable = true;
        //Debug.Log("Node Unlocked");
    }

    /// <summary>
    /// This handles the logic for purchasing the node
    /// </summary>
    private void PurchaseNode()
    {
        //Checks with the skill tree manager to see if it can be purchased
        if (skillTreeManager.CanPurchaseNode(cost))
        {
            Status = NodeStatus.Purchased;
            button.interactable = false;
            //Debug.Log("Node Purchased");

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

    /// <summary>
    /// This permanantly locks the node.
    /// </summary>
    public void PermanantlyLockNode()
    {
        LockNode();
        isPermaLocked = true;
    }

    /// <summary>
    /// This triggers when the public event SkillTreeNodePurchased() is called.
    /// Unlocks the node if it can be unlocked
    /// </summary>
    private void AnySkillTreeNodePurchased()
    {
        //checks to see if its currently locked and is not permalocked
        if (status == NodeStatus.Locked && !isPermaLocked)
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
                PurchaseNode();
                break;
            case NodeStatus.Purchased:
                Debug.Log("You've already purchased this node!");
                break;
            default:
                Debug.Log("Tyler update the fucking switch statement in clickedon() in skilltreenode - missing cases");
                break;
        }
    }

    #endregion

    #endregion
}
