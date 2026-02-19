/*************************************************
Author Names : 		Tyler Bouchard
Date Created : 		2/2/2026
Date Last Modified : 2/10/2026
Brief Description : this is the behavior of an artifact node, this gets spawned when you 
click on the Notebook Artifact slot
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.Cinemachine.CinemachineOrbitalTransposer;

public class ArtifactNodeBehavior : MonoBehaviour
{
    private RectTransform rectTransform;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public ArtifactData artifactData;

    private SlotBehavior slotBehavior;
    [HideInInspector] public NotebookArtifactNodeBehavior notebookArtifactNode;

    private bool dragging = true;
    private Vector2 offset;
    private Vector2 mPos;
    ArtifactMenuManager artifactManager;

    /// <summary>
    /// initialization
    /// </summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        artifactManager = FindFirstObjectByType<ArtifactMenuManager>();

        Debug.Log(artifactData.Name);
        artifactManager.ArtifactPickedUp(artifactData);
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
            transform.parent = null;
            dragging = true;

            UIAudioManager.Instance.UIPickUp(transform);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mPos, canvas.worldCamera, out offset);
            if (slotBehavior != null)
            {
                slotBehavior.artifact = null;
            }
        }
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
    /// Sets holding to false
    /// </summary>
    private void LeftClickReleased()
    {
        if (IsPointerOverThisUI())
        {

            dragging = false;

            GameObject slot = ArtifactOverSnapLocation();
            if (slot != null)
            {
                rectTransform.position = slot.GetComponent<RectTransform>().position;
                slotBehavior = slot.GetComponent<SlotBehavior>();
                slotBehavior.artifact = artifactData;
                slot.GetComponent<EquippedArtifactButton>()?.ButtonClicked();

                UIAudioManager.Instance.UIDrop(transform);

                transform.parent = GameObject.Find("NewOutOfCombatMenu").transform;
            }
            else
            {
                notebookArtifactNode.Equip(false);
                Destroy(gameObject);
            }

        }

    }

    /// <summary>
    /// controls the click and drag functionality
    /// </summary>
    private void Update()
    {

        //what happens the its being dragged (mouse button is held)
        if (dragging)
        {
            notebookArtifactNode.Equip(true);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform.parent as RectTransform, Input.mousePosition, canvas.worldCamera, out localPoint);
            rectTransform.localPosition = localPoint - offset;
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