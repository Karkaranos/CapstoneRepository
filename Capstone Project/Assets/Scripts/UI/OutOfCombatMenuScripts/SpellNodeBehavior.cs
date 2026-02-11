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
    [HideInInspector] public RuneData runeData;

    private SlotBehavior slotBehavior;
    [HideInInspector] public NotebookSpellNodeBehavior notebookSpellNode;

    private bool dragging = true;
    private Vector2 offset;

    /// <summary>
    /// initialization
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// controls the click and drag functionality
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverThisUI())
            {
                dragging = true;
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out offset);
                if (slotBehavior != null)
                {
                    slotBehavior.rune = null;
                }
            }
        }
        if (dragging && Input.GetMouseButton(0))
        {
            notebookSpellNode.Equip(true);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint - offset;
        }
        if (Input.GetMouseButtonUp(0) && dragging)
        {
            dragging = false;
           
            GameObject slot = SpellOverSnapLocation();
            if (slot != null)
            {
                rectTransform.position = slot.GetComponent<RectTransform>().position;
                slotBehavior = slot.GetComponent<SlotBehavior>();
                slotBehavior.rune = runeData;
                
            }
            else {
                notebookSpellNode.Equip(false);
                Destroy(gameObject);
            }
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