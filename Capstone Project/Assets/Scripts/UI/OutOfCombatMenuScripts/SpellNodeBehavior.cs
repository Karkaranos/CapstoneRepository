/*************************************************
Author Names : 		Tyler Bouchard, Cade Naylor, Clare Grady
Date Created : 		2/2/2026
Date Last Modified : 2//2026
Brief Description : this is the behavior of an spell node, this gets spawned when you 
click on the Notebook spell slot
***************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellNodeBehavior : MonoBehaviour
{
    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public RuneData runeData;

    private SlotBehavior slotBehavior;
    [HideInInspector] public NotebookSpellNodeBehavior notebookSpellNode;

    private bool dragging = true;
    private Vector2 offset;
    private bool holding = false;
    private Vector2 mPos;
    private bool locationSet = false;
    private SkillTreeManager skillTreeManager;

    /// <summary>
    /// initialization
    /// </summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();

        skillTreeManager.SelectNode(runeData);

    }


    /// <summary>
    /// Assigns event listeners on enable
    /// </summary>
    private void OnEnable()
    {
        PublicEvents.LeftClicked += LeftClickStarted;
        PublicEvents.LeftClickReleased += LeftClickReleased;
        PublicEvents.MousePosition += GetMousePos;
    }

    /// <summary>
    /// Assigns event listeners on enable
    /// </summary>
    private void OnDisable()
    {
        PublicEvents.LeftClicked -= LeftClickStarted;
        PublicEvents.LeftClickReleased -= LeftClickReleased;
        PublicEvents.MousePosition -= GetMousePos;
    }

    /// <summary>
    /// Sets holding to true
    /// </summary>
    private void LeftClickStarted()
    {
        if (IsPointerOverThisUI())
        {
            //transform.parent = null;
            holding = true;
            dragging = true;

            UIAudioManager.Instance.UIPickUp(transform);

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mPos, canvas.worldCamera, out offset);

            rectTransform.anchoredPosition = mPos;

            

            PublicEvents.RuneUnequipped?.Invoke(runeData);
            if (slotBehavior != null)
            {
                slotBehavior.rune = null;
                
            }
        }
        else
        {
            GetComponent<Image>().raycastTarget = false;
        }
    }

    /// <summary>
    /// Sets holding to false
    /// </summary>
    private void LeftClickReleased()
    {
       
        if (IsPointerOverThisUI())
        {
            holding = false;

            dragging = false;

            GameObject slot = SpellOverSnapLocation();
            
            if (slot != null)
            {
                rectTransform.position = slot.GetComponent<RectTransform>().position;
                slotBehavior = slot.GetComponent<SlotBehavior>();

                if (slotBehavior.rune == null)
                {
                    slotBehavior.rune = runeData;
                    slotBehavior.heldSpellObject = this;
                    //slot.GetComponent<EquippedSpellNode>()?.OnClick();
                    FindFirstObjectByType<SkillAndArtifactManager>().SetIndexOfEquippedSpells(slot.GetComponent<EquippedSpellNode>().index, runeData);

                    UIAudioManager.Instance.UIDrop(transform);

                    //transform.parent = GameObject.Find("NewOutOfCombatMenu").transform;
                    //transform.SetParent(slot.transform);


                }
                else
                {
                    PublicEvents.RuneUnequipped?.Invoke(slotBehavior.heldSpellObject.runeData);
                    slotBehavior.heldSpellObject.notebookSpellNode.Equip(false);
                    Destroy(slotBehavior.heldSpellObject);

                    slotBehavior.rune = runeData;
                    slotBehavior.heldSpellObject = this;
                    //slot.GetComponent<EquippedSpellNode>()?.OnClick();
                    FindFirstObjectByType<SkillAndArtifactManager>().SetIndexOfEquippedSpells(slot.GetComponent<EquippedSpellNode>().index, runeData);

                    UIAudioManager.Instance.UIDrop(transform);

                    //transform.parent = GameObject.Find("NewOutOfCombatMenu").transform;
                    //transform.SetParent(slot.transform);
                }

                
                
            }
            else
            {
                notebookSpellNode.Equip(false);
                Destroy(gameObject);
            }

        }
        GetComponent<Image>().raycastTarget = true;
    }

    /// <summary>
    /// Constantly gets the mouse position
    /// </summary>
    /// <param name="m"></param>
    private void GetMousePos(Vector2 m)
    {
        mPos = m;
    }

    /// <summary>
    /// controls the click and drag functionality
    /// </summary>
    private void Update()
    {
       
        if (dragging)
        {
            notebookSpellNode.Equip(true);
            Vector2 localPoint = Vector2.zero;

            try
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, mPos, canvas.worldCamera, out localPoint);

            }
            catch
            {
               
            }

            rectTransform.localPosition = localPoint - offset;
            //to fix a bug there they appear off the mouse i turned this off by default that why this here
            GetComponent<Image>().color = Color.white;
        }
        
    }

    /// <summary>
    /// returns true if the mouse is over the game object this script is attached to
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverThisUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = (Vector3)mPos;


        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject)
                return true;
        }

        return false;
    }

    /// <summary>
    /// if this gameObject is over a slot that it is suposed to snap to, this will snap it to it
    /// </summary>
    /// <returns></returns>
    private GameObject SpellOverSnapLocation()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = (Vector3)mPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            SlotBehavior sb = result.gameObject.GetComponent<SlotBehavior>();
            if (sb && sb.rune == null && sb.isSpellSlot()) {
                return result.gameObject;
            }
        }
        return null;
    }
}