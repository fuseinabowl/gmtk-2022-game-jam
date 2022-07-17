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
    [SerializeField]
    private DiceTurnController diceTurnController = null;
    [SerializeField]
    private AudioSource audioSource = null;

    [SerializeField]
    private List<AudioClip> typingSounds = new List<AudioClip>();
    [SerializeField]
    private AnimationCurve typingRandomVolume = AnimationCurve.Linear(0f,0.5f, 1f,1f);

    private float popUpOpacity = 0.0f;
    private bool shouldShowPopUp = false;

    private string textToType;
    private bool typingText = false;
    private float timeToTypeText = 0.0f;
    private int currentCharactersTyped = 0;

    private string[] storyTexts = new string[] {
@"You must now help me
cheat at dice to damn other
poor souls.",
@"Grab and throw the dice 
above the tray to generate 
your available moves.",
@"Then click and drag anywhere 
on the big dice to fling yourself
in that direction. ",
    };
	
	[SerializeField]
	[TextArea]
    private string[] storyTexts2 = new string[] {};
	
    private int currentStoryString = 0;
    private int currentStoryChapter = 0;

    // Start is called before the first frame update
    void Start()
    {
        RegisterWithOnNewTurn();
        MakeContinueButtonHideUI();
        SetOpacity(0.0f);
        ShowNextStoryPopUp();
    }
    
    private void RegisterWithOnNewTurn()
    {
        diceTurnController.onNewTurn += OnNewTurn;
    }

    private void OnNewTurn()
    {
        ShowNextStoryPopUp();
    }

    private void ShowNextStoryPopUp()
    {
        if (IsInStoryMode())
        {
            var currentChapterTexts = GetCurrentChapterTexts();

            if (currentStoryString < currentChapterTexts.Length)
            {
                ShowPopUp();
                SetTextToType(currentChapterTexts[currentStoryString]);
                currentStoryString += 1;
            }
        }
    }

    private string[] GetCurrentChapterTexts()
    {
        switch (currentStoryChapter)
        {
            case 0:
            {
                return storyTexts;
            }
            case 1:
            {
                return storyTexts2;
            }
            default:
            {
                return null;
            }
        }
    }

    private bool IsInStoryMode()
    {
        return currentStoryChapter < 2;
    }

    private void MakeContinueButtonHideUI()
    {
        GetContinueButton().clicked += () =>
        {
            if (IsInStoryMode() && currentStoryString < GetCurrentChapterTexts().Length)
            {
                ShowNextStoryPopUp();
            }
            else
            {
                Debug.Log($"Upping chapter {currentStoryChapter}");
                currentStoryChapter++;
                currentStoryString = 0;

                HidePopUp();
                SetTextToType(""); 
            }
        };
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

            if (currentCharactersTyped < charactersToType)
            {
                PlayTypingSounds(charactersToType - currentCharactersTyped);
                currentCharactersTyped = charactersToType;
            }
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

    public bool IsShown()
    {
        return shouldShowPopUp;
    }

    public bool IsFullyShown()
    {
        return popUpOpacity >= 1.0f;
    }

    void SetTextToType(string text)
    {
        timeToTypeText = 0.0f;
        currentCharactersTyped = 0;
        typingText = true;
        textToType = text;
        SetText("");
    }

    private void PlayTypingSounds(int sounds)
    {
        for (int i = 0; i < sounds; i++)
        {
            var selectedSoundIndex = Random.Range(0, typingSounds.Count - 1);
            var selectedSound = typingSounds[selectedSoundIndex];

            audioSource.pitch = Mathf.Lerp(0.75f, 1.25f, Random.value);
            float volume = typingRandomVolume.Evaluate(Random.value);
            audioSource.PlayOneShot(selectedSound, volume);
        }
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

    private Button GetContinueButton()
    {
        return storyPopUpUI.rootVisualElement.Q<Button>("ContinueButton");
    }
}
