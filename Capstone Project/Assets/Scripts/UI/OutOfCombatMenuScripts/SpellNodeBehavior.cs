/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this is the behavior of an spell node, this gets spawned when you 
click on the Notebook spell slot
***************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellNodeBehavior : MonoBehaviour
{
    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
     public RuneData runeData;

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

        Debug.Log(runeData.RuneName);
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
            holding = true;
            dragging = true;

                UIAudioManager.Instance.UIPickUp(transform);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mPos, canvas.worldCamera, out offset);
                if (slotBehavior != null)
                {
                    slotBehavior.rune = null;
                }
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
                slotBehavior.rune = runeData;
                slot.GetComponent<EquippedSpellNode>()?.OnClick();

                UIAudioManager.Instance.UIDrop(transform);
            }
            else
            {
                notebookSpellNode.Equip(false);
                Destroy(gameObject);
            }

        }

    }

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
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, mPos, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint - offset;
        }
        
    }

    /// <summary>
    /// returns true if the mouse is over the game object this script is attached to
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverThisUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

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
        pointerData.position = Input.mousePosition;

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