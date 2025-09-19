using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;



public class NaughtyAttributesDemo : MonoBehaviour
{
    #region SETUP
    //Generic Enum - Does not do anything in code, merely exists so design can choose what variables
    //to look at
    [System.Serializable] public enum Settings 
    {
        WeaponSettings,
        PlayerSettings,
        OtherSettings,
        Dropdowns
    }

    //test class for this demo
    [System.Serializable]
    public class naughtyTestClass
    {
        //normal variable
        public int num;

        //Required - pops up with an error if var is null
        //AllowNesting - allows naughtyattributes to function in nested gameobjects
        [Required("Prefab is Required"), AllowNesting]
        public GameObject prefab;
    }

    //the control center of the inspector - does absolutely nothing in code
    [SerializeField] private Settings currentInspectorShowing;

    #endregion SETUP

    #region WEAPON SETTINGS

    //I have a line in front of each setting type to further indicate what setting you are working in
    //      Note - HorizontalLine can only go above variables
    [HorizontalLine(4, EColor.Red)]

    //showif - only shows if the value input is true
    //      has counterpart HideIf - does the same thing but hides instead
    //      this is set up so the variable only shows if the settings enum is set to 
    //      weapon settings. Hides any other time.
    //nameof - gets the exact string name of the input variable/function.
    //      Super helpful for preventing typos, and if you ever need to change the name
    //      of a function/variable, this also changes with it
    [ShowIf(nameof(currentInspectorShowing), Settings.WeaponSettings), SerializeField]
    private bool weaponPower;

    #endregion WEAPON SETTINGS

    #region PLAYER SETTINGS

    //I have a line in front of each setting type to further indicate what setting you are working in
    //      Note - HorizontalLine can only go above variables
    [HorizontalLine(4, EColor.Blue)]

    //showif - only shows if the value input is true
    //      has counterpart HideIf - does the same thing but hides instead
    //      this is set up so the variable only shows if the settings enum is set to 
    //      weapon settings. Hides any other time.
    //nameof - gets the exact string name of the input variable/function.
    //      Super helpful for preventing typos, and if you ever need to change the name
    //      of a function/variable, this also changes with it
    //OnValueChanged - when the value is changed in the inspector, triggers the function
    //      that is put in as a parameter. Takes a string input so I use nameof
    //ValidateInput - sort of like OnValueChanged but requires the function to return a bool. If the
    //      bool is true, does nothing. If the bool is false, throws a warning in the inspector itself.
    //      Can have custom warning text, like I do here
    [ShowIf(nameof(currentInspectorShowing), Settings.PlayerSettings), SerializeField, 
        OnValueChanged(nameof(OnSpeedChanged)), ValidateInput(nameof(IsPositive), "Player Speed Must Be Positive")]
    private float playerSpeed;

    //ProgressBar - Alternative to readonly - shows numbers as percentages kinda
    //      Not super useful, as you can't edit the variable in the inspector anymore
    [ShowIf(nameof(currentInspectorShowing), Settings.PlayerSettings), SerializeField, 
        ProgressBar("Health", 200, EColor.Red)]
    private int health = 60;

    [Space(4)]

    //MinMaxSlider - self explanatory. Limits what can be put in the inspector to within the min and max 
    //      while also adding a cool bar
    [ShowIf(nameof(currentInspectorShowing), Settings.PlayerSettings), SerializeField, MinMaxSlider(0, 100)]
    private Vector2Int playerMaxHealth;

    #endregion PLAYER SETTINGS

    #region OTHER SETTINGS

    [HorizontalLine(4, EColor.Indigo)]

    //showif - only shows if the value input is true
    //      has counterpart HideIf - does the same thing but hides instead
    //      this is set up so the variable only shows if the settings enum is set to 
    //      weapon settings. Hides any other time.
    //nameof - gets the exact string name of the input variable/function.
    //      Super helpful for preventing typos, and if you ever need to change the name
    //      of a function/variable, this also changes with it
    //InfoBox - has a box always active with info. Not particularly useful, use tooltips instead
    [ShowIf(nameof(currentInspectorShowing), Settings.OtherSettings), SerializeField, InfoBox("Do Not Mess With This Please", EInfoBoxType.Warning)]
    private string errorOutput;

    [Space(4)]

    //Dropdown - limits the variable's possible values to the list/array provided. Could be useful for
    //      designer testing - streamlining choosing which item to equip? idk its cool but a bit of 
    //      a stretch to use regularly
    [ShowIf(nameof(currentInspectorShowing), Settings.OtherSettings), SerializeField, Dropdown(nameof(stringVals))]
    private string animalName;

    //list of possible values for animalName variable
    private List<string> stringVals = new List<string>() { "dog", "cat", "hawk" };

    #endregion OTHER SETTINGS

    #region DROPDOWNS

    //I have a line in front of each setting type to further indicate what setting you are working in
    //      Note - HorizontalLine can only go above variables
    [HorizontalLine(3, EColor.Green)]

    //Tag - limits values to available tags
    [ShowIf(nameof(currentInspectorShowing), Settings.Dropdowns), SerializeField, Tag] private string tagToLookFor;

    //Layer - limits values to available layers
    [ShowIf(nameof(currentInspectorShowing), Settings.Dropdowns), SerializeField, Layer] private string layerToLookFor;

    //Scene - limits values to scenes in build index
    [ShowIf(nameof(currentInspectorShowing), Settings.Dropdowns), SerializeField, Scene] private string sceneToLookFor;

    #endregion DROPDOWNS

    #region OTHER FUN STUFF

    //Adds all variables with the same foldout name to an indented dropdown
    [Foldout("Other Fun Stuff"), SerializeField] private string objName;

    //allownesting and required test - see above
    [Foldout("Other Fun Stuff"), SerializeField] private naughtyTestClass tClass;

    #endregion OTHER FUN STUFF

    #region BUTTONS


    [Foldout("Refs"), SerializeField] private GameObject cube;
    private List<GameObject> cubes;

    
    //Button - arguably the best feature of NaughtyAttributes. 
    //      Triggers function when you click button in inspector
    [Button("Spawn Cube")]
    private void InsCube()
    {
        cubes.Add(Instantiate(cube));
        
    }

    [Button("Delete Cubes")]

    private void DelCubes()
    {
        foreach (GameObject c in cubes)
        {
            DestroyImmediate(c);
        }
        cubes.Clear();
    }

    #endregion BUTTONS

    #region HELPER FUNCS

    private void OnSpeedChanged()
    {
        health = Mathf.RoundToInt(2 * playerSpeed);
    }

    private bool IsPositive(float i)
    {
        return i >= 0;
    }

    #endregion HELPER FUNCS
}
