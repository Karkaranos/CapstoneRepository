////////////////////////////////////////////////
///Base class for everything the player can click on
///Has only virtual funcs and no variables
///
///written by Tyler Hayes, 10/31/2024
////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    /// <summary>
    /// runs this when this gameobject is clicked on
    /// </summary>
    public virtual void ClickedOn()
    {

    }

    /// <summary>
    /// runs this when the player's cursor is hovering over the gameobject
    /// </summary>
    public virtual void OnHover()
    {

    }

    /// <summary>
    /// runs this when the player's cursor is no longer hovering over the gameobject
    /// </summary>
    public virtual void OnHoverLeave()
    {

    }
}