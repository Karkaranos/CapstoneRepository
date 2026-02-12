/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this is the behavior of an artifact node, this gets spawned when you 
click on the Notebook Artifact slot
***************************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ArtifactNodeBehavior : MonoBehaviour
{
    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public ArtifactData artifactData;

    private SlotBehavior slotBehavior;
    [HideInInspector] public NotebookArtifactNodeBehavior notebookArtifactNode;

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
        //what happens when its been clicked on (the one frame of the click)
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverThisUI())
            {
                dragging = true;


                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvas.worldCamera, out offset);
                if (slotBehavior != null)
                {
                    slotBehavior.artifact = null;
                }
            }
        }

        //what happens the its being dragged (mouse button is held)
        if (dragging && Input.GetMouseButton(0))
        {
            notebookArtifactNode.Equip(true);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint - offset;
        }

        //what happens when you lot go of the mouse
        if (Input.GetMouseButtonUp(0) && dragging)
        {
            dragging = false;

            // if its over its slot it snaps to it and updates the slots SlotBehavior
            GameObject slot = ArtifactOverSnapLocation();
            if (slot != null)
            {
                rectTransform.position = slot.GetComponent<RectTransform>().position;
                slotBehavior = slot.GetComponent<SlotBehavior>();
                slotBehavior.artifact = artifactData;
            }
            else
            {
                notebookArtifactNode.Equip(false);
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// returns true if the mouse is ober the gameObject that this  script is attatched to
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
    private GameObject ArtifactOverSnapLocation()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            SlotBehavior sb = result.gameObject.GetComponent<SlotBehavior>();
            if (sb && sb.artifact == null && sb.isArtifactSlot())
            {
                return result.gameObject;
            }
        }
        return null;
    }
}