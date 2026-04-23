using NaughtyAttributes;
using System;
using UnityEngine;

public class BookBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject bookanimObj;
    private Animator bookanim;
    [Tooltip("A reference to the book's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer bSprite;
    private RuneEvents re;
    #endregion

    #region Functions
    /// <summary>
    /// Locates all references of the book's animator
    /// </summary>
    void Start()
    {
        bookanim = bookanimObj.GetComponent<Animator>();
        re = FindAnyObjectByType<RuneEvents>(FindObjectsInactive.Exclude);
        re.AssignBookAnim(bookanim);
    }

    /// <summary>
    /// Enables BookAnimation when ReadyClicked event is started
    /// </summary>
    public void OnEnable()
    {
        PublicEvents.ReadyClicked += BookAnimation;
    }

    /// <summary>
    /// Disables BookAnimation when ReadyClicked event is started
    /// </summary>
    public void OnDisable()
    {
        PublicEvents.ReadyClicked -= BookAnimation;
    }

    /// <summary>
    /// Cycles through the animations within the book's animators, 
    /// fulfilling conditions needed to initialize each one
    /// </summary>
    private void BookAnimation()
    {
        bookanim.SetBool("Fly", true);
        bookanim.SetBool("Wait", false);
        bookanim.SetBool("Idle", true);
    }
    #endregion
}
