using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellNodeBehavior : MonoBehaviour
{
    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public RuneData runeData;

    private SlotBehavior slotBehavior;
    [HideInInspector] public NotebookSpellNodeBehavior notebookSpellNode;

    public bool unlocked = true;
    private bool draggable = true;
    private bool dragging = true;

    
    private Vector2 offset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && draggable)
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

    private GameObject SpellOverSnapLocation()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            SlotBehavior sb = result.gameObject.GetComponent<SlotBehavior>();
            if (sb && sb.rune == null) {
                return result.gameObject;
            }
        }
        return null;
    }
}
