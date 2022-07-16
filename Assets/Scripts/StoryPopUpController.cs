using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryPopUpController : MonoBehaviour
{

    [SerializeField]
    private float opacityTimeToFull = 2.0f;
    [SerializeField]
    private float characterTypingRatePerSecond = 20.0f;
    [SerializeField]
    private UIDocument storyPopUpUI = null;

    private float popUpOpacity = 0.0f;
    private bool shouldShowPopUp = false;

    private string textToType;
    private bool typingText = false;
    private float timeToTypeText = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //ShowPopUp();
    }

    // Update is called once every 1/50 seconds
    void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        float opacityChangeRate = 1 / opacityTimeToFull;
        float opacityChangeAmount = opacityChangeRate * deltaTime;
        if (shouldShowPopUp && popUpOpacity < 1.0f)
        {
            popUpOpacity = Mathf.Min(popUpOpacity + opacityChangeAmount, 1.0f);
            SetOpacity(popUpOpacity);
        }
        else if (!shouldShowPopUp && popUpOpacity > 0.0f)
        {
            popUpOpacity = Mathf.Max(popUpOpacity - opacityChangeAmount, 0.0f);
            SetOpacity(popUpOpacity);
        }

        if (typingText)
        {
            int charactersInCurrentString = textToType.Length;
            timeToTypeText += deltaTime;

            int charactersToType = (int) (timeToTypeText * characterTypingRatePerSecond);
            SetText(textToType.Substring(0, charactersToType));
            typingText = charactersToType < charactersInCurrentString;
        }

        if (popUpOpacity == 1.0f)
        {
            HidePopUp();
        }
        else if (!IsShown())
        {
           ShowPopUp();
           SetTextToType("Listen here you little shit, I am the devil and you will do what I say now get in the goddamn box");
        }
    }

    void ShowPopUp()
    {
        shouldShowPopUp = true;
    }
      
    void HidePopUp()
    {
        shouldShowPopUp = false;
    }

    bool IsShown()
    {
        return popUpOpacity > 0.0f;
    }

    bool IsFullyShown()
    {
        return popUpOpacity >= 1.0f;
    }

    void SetTextToType(string text)
    {
        timeToTypeText = 0.0f;
        typingText = true;
        textToType = text;
        SetText("");
    }

    private void SetOpacity(float opacity)
    {
        GetStoryPopUpVisualElement().style.opacity = opacity;
    }

    private void SetText(string text)
    {
        GetLabel().text = text;
    }

    private VisualElement GetStoryPopUpVisualElement()
    {
        return storyPopUpUI.rootVisualElement.Q<VisualElement>("StoryPopUp");
    }

     private Label GetLabel()
    {
        return storyPopUpUI.rootVisualElement.Q<Label>("StoryText");
    }
}
