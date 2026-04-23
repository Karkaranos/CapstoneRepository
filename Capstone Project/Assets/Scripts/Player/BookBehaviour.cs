using NaughtyAttributes;
using System;
using UnityEngine;

public class BookBehaviour : MonoBehaviour
{
    [SerializeField, Tooltip("A reference to the object the Animator is on")] private GameObject bookanimObj;
    private Animator bookanim;
    [Tooltip("A reference to the book's Sprite Renderer"), SerializeField, Required]
    private SpriteRenderer bSprite;
    private RuneEvents re;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bookanim = bookanimObj.GetComponent<Animator>();
        re = FindAnyObjectByType<RuneEvents>(FindObjectsInactive.Exclude);
        re.AssignBookAnim(bookanim);
    }

    public void OnEnable()
    {
        PublicEvents.ReadyClicked += BookAnimation;
    }

    public void OnDisable()
    {
        PublicEvents.ReadyClicked -= BookAnimation;
    }

    private void BookAnimation()
    {
        bookanim.SetBool("Fly", true);
        bookanim.SetBool("Wait", false);
        bookanim.SetBool("Idle", true);
    }
}
