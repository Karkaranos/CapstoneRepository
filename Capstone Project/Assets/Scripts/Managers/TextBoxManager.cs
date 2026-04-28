/******************************************************************************
 * Author: Brad Dixon
 * Creation Date: 4/2/2026
 * Last Modified: 4/13/2026
 * Brief: Controls the content and the popping up of text boxes. 
 * Also makes sure the text is confined to the box
 * External Resources: N/A
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class TextBoxManager : MonoBehaviour
{
    const float LEFT_PADDING_PERCENTAGE = .13f;
    const float RIGHT_PADDING_PERCENTAGE = .11f;
    const float VERTICAL_PADDING_PERCENTAGE = .1f;

    PlayerInput pInput;
    InputAction click;

    [SerializeField] private bool canClick;
    public bool CanClick
    {
        get => canClick;
        set => canClick = value;
    }
    [SerializeField] bool showFirstBoxOnStart;


    [SerializeField] List<TextBox> textBoxes = new List<TextBox>();

    [Tooltip("How fast the text appears.")]
    [SerializeField] private float textSpeed;
    [Tooltip("How long the text should wait before going to the next one if it auto continues.")]
    [SerializeField] private float autoTextLingerTime;
    [Tooltip("Will test the textbox at this index when you click the Test Text Box Button.")]
    [SerializeField] private int textBoxTestingIndex;
    private int textBoxIndex;

    [SerializeField] private GameObject textBoxImage;
    private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private TMP_Text textBoxText;
    private RectTransform rectForm;

    private bool allTextPresent;

    [HideInInspector] public bool inTutorial;
    [HideInInspector] public int tutorialCheck;

    [Button("Test Text Box")]
    /// <summary>
    /// Test button to see if the size of the textbox works.
    /// </summary>
    private void TestTextBoxSize()
    {
        layoutGroup = textBoxImage.GetComponent<HorizontalLayoutGroup>();
        textBoxIndex = textBoxTestingIndex;
        rectForm = textBoxImage.GetComponent<RectTransform>();
        ShowTextBox();
    }

    [Button("Test Clicking")]
    /// <summary>
    /// Test button that simulates clicking a text box.
    /// </summary>
    private void TextClick()
    {
        ClickTextBox();
    }

    /// <summary>
    /// Initializes the input system
    /// </summary>
    private void Awake()
    {
        pInput = GetComponent<PlayerInput>();
        pInput.currentActionMap.Enable();
        click = pInput.currentActionMap.FindAction("Click");
    }

    /// <summary>
    /// Enables the click function
    /// </summary>
    private void OnEnable()
    {
        click.started += Click_started;
    }

    /// <summary>
    /// Disables the click function
    /// </summary>
    private void OnDisable()
    {
        click.started -= Click_started;
    }

    /// <summary>
    /// Moves through the text boxes when the left mouse button is clicked
    /// </summary>
    /// <param name="obj"></param>
    private void Click_started(InputAction.CallbackContext obj)
    {
        if(canClick)
        {
            ClickTextBox();
        }
    }

    /// <summary>
    /// Sets the index to 0 and sets components
    /// </summary>
    private void Start()
    {
        textBoxIndex = 0;
        tutorialCheck = 0;
        layoutGroup = textBoxImage.GetComponent<HorizontalLayoutGroup>();
        rectForm = textBoxImage.GetComponent<RectTransform>();

        if(showFirstBoxOnStart)
        {
            canClick = true;
            ShowTextBox();
        }
    }
    
    /// <summary>
    /// Sets the textbox size and padding. Also shows the text for the text box.
    /// </summary>
    public void ShowTextBox()
    {
        textBoxImage.SetActive(true);
        textBoxImage.transform.position = new Vector3(textBoxes[textBoxIndex].TextboxLocation.x,
            textBoxes[textBoxIndex].TextboxLocation.y, transform.position.z);
        textBoxText.fontSize = textBoxes[textBoxIndex].FontSize;
        rectForm.sizeDelta = new Vector2(textBoxes[textBoxIndex].HorizontalSize, textBoxes[textBoxIndex].VerticleSize);

        int leftPadding = (int) (rectForm.rect.width * LEFT_PADDING_PERCENTAGE);
        int rightPadding = (int)(rectForm.rect.width * RIGHT_PADDING_PERCENTAGE);

        int verticlePadding = (int)(rectForm.rect.height * VERTICAL_PADDING_PERCENTAGE);

        layoutGroup.padding.left = leftPadding;
        layoutGroup.padding.right = rightPadding;
        layoutGroup.padding.top = verticlePadding;
        layoutGroup.padding.bottom = verticlePadding;

        allTextPresent = false;

        if(textBoxIndex == 0)
        {
            FindFirstObjectByType<NotebookManager>().CreateTutorialReference(this);
        }
        StartCoroutine(DisplayText());
    }

    /// <summary>
    /// When the mouse is clicked, will go to the next text box or show all the text
    /// </summary>
    private void ClickTextBox()
    {
        if(allTextPresent)
        {
            ++textBoxIndex;
            textBoxText.text = "";
            if (textBoxes[textBoxIndex - 1].GoToNextTextbox)
            {
                ShowTextBox();
            }
            else
            {
                textBoxImage.SetActive(false);
                canClick = false;
            }
        }
        else
        {
            StopAllCoroutines();
            textBoxText.text = textBoxes[textBoxIndex].TextboxContent;
            allTextPresent = true;
            if(textBoxes[textBoxIndex].disableClick)
            {
                canClick = false;
                ++textBoxIndex;
                ++tutorialCheck;
            }
        }
    }



    /// <summary>
    /// Displays the text one letter at a time so you can watch it appear
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayText()
    {
        string displayedText = "";
        int textContentIndex = 0;

        while(!displayedText.Equals(textBoxes[textBoxIndex].TextboxContent))
        {
            displayedText = textBoxes[textBoxIndex].TextboxContent.Substring(0, textContentIndex);
            yield return new WaitForSeconds(textSpeed);
            textBoxText.text = displayedText;
            ++textContentIndex;
        }

        if (textBoxes[textBoxIndex].disableClick)
        {
            canClick = false;
            ++tutorialCheck;
        }

        allTextPresent = true;
    }
}
