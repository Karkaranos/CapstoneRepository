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

public class TextBoxManager : MonoBehaviour
{
    const float LEFT_PADDING_PERCENTAGE = .13f;
    const float RIGHT_PADDING_PERCENTAGE = .11f;
    const float VERTICAL_PADDING_PERCENTAGE = .1f;

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
    /// Sets the index to 0 and sets components
    /// </summary>
    private void Start()
    {
        textBoxIndex = 0;
        layoutGroup = textBoxImage.GetComponent<HorizontalLayoutGroup>();
        rectForm = textBoxImage.GetComponent<RectTransform>();
    }
    
    /// <summary>
    /// Sets the textbox size and padding. Also shows the text for the text box.
    /// </summary>
    private void ShowTextBox()
    {
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
        StartCoroutine(DisplayText(textBoxes[textBoxIndex].GoToNextTextbox));
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
            ShowTextBox();
        }
        else
        {
            StopAllCoroutines();
            textBoxText.text = textBoxes[textBoxIndex].TextboxContent;
            allTextPresent = true;
        }
    }

    /// <summary>
    /// Displays the text one letter at a time so you can watch it appear
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayText(bool autoContinue)
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

        allTextPresent = true;
        if(autoContinue)
        {
            StartCoroutine(TimeBeforeNextTextBox());
        }
    }

    /// <summary>
    /// How long the code should wait when auto continuing
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeBeforeNextTextBox()
    {
        yield return new WaitForSeconds(autoTextLingerTime);
        ClickTextBox();
    }
}
