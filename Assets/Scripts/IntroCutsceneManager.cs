using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class IntroCutsceneManager : MonoBehaviour
{
    [SerializeField]
    private UIDocument introCutsceneUI = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CutsceneSequence());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            StopAllCoroutines();

            SceneManager.LoadScene("MainGame");
        }
    }

    IEnumerator FadeIn(VisualElement element, float opacityTimeToFull)
    {
        float startTime = Time.time;
        float currentTime = startTime;
        do
        {
            yield return null;
            currentTime = Time.time;
            SetOpacity(element, Mathf.Lerp( 0.0f, 1.0f, (currentTime - startTime) / opacityTimeToFull));
        } 
        while( currentTime - startTime < opacityTimeToFull );
    }

    IEnumerator FadeOut(VisualElement element, float opacityTimeToGone)
    {
        float startTime = Time.time;
        float currentTime = startTime;
        do
        {
            yield return null;
            currentTime = Time.time;
            SetOpacity(element, Mathf.Lerp( 0.0f, 1.0f, 1.0f - ((currentTime - startTime) / opacityTimeToGone)));
        } 
        while( currentTime - startTime < opacityTimeToGone );
    }

    IEnumerator FadeInAndOut(VisualElement element, float opacityChangeInTime, float holdTime, float opacityChangeOutTime)
    {
        yield return StartCoroutine(FadeIn(element, opacityChangeInTime));
        yield return new WaitForSeconds(holdTime);
        yield return StartCoroutine(FadeOut(element, opacityChangeOutTime));
    }

    IEnumerator ChangeTextInAndOut(Label textLabel, float opacityChangeInTime, float holdTime, float opacityChangeOutTime, string text)
    {
        textLabel.text = text;
        yield return StartCoroutine(FadeInAndOut(textLabel, opacityChangeInTime, holdTime, opacityChangeOutTime));
    }

    IEnumerator CutsceneSequence()
    {
        var devilImage = GetDevilImage();
        var diceRollImage = GetDiceRollImage();
        var badLuckImage = GetBadLuckImage();
        var textLabel = GetCutsceneText();
        var uiBackground = GetUIBackGround();

        SetOpacity(devilImage, 0.0f);
        SetOpacity(diceRollImage, 0.0f);
        SetOpacity(badLuckImage, 0.0f);
        SetOpacity(textLabel, 0.0f);
        SetOpacity(uiBackground, 1.0f);

        float standardFadeIn = 2.0f;
        float standardFadeOut = 1.0f;
        float standardHoldTime = 2.0f;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(ChangeTextInAndOut(textLabel, standardFadeIn, standardHoldTime, standardFadeOut,
            "Jack, roll your last dice, betting everything on this..."));

        yield return StartCoroutine(FadeInAndOut(diceRollImage, standardFadeIn, standardHoldTime, standardFadeOut));

        yield return StartCoroutine(FadeInAndOut(badLuckImage, standardFadeIn, standardHoldTime, standardFadeOut));

        yield return StartCoroutine(FadeIn(devilImage, standardFadeIn));

        yield return StartCoroutine(ChangeTextInAndOut(textLabel, standardFadeIn, standardHoldTime, standardFadeOut, 
            "Looks like your soul...\nIs Mine"));

        yield return StartCoroutine(FadeOut(devilImage, standardFadeOut));

        SceneManager.LoadScene("MainGame");
    }

    void SetOpacity(VisualElement element, float opacity)
    {
        element.style.opacity = opacity;
    }

    private VisualElement GetUIBackGround()
    {
        return introCutsceneUI.rootVisualElement.Q<VisualElement>("BackGround");
    }

    private VisualElement GetDevilImage()
    {
        return introCutsceneUI.rootVisualElement.Q<VisualElement>("DevilImage");
    }

    private VisualElement GetDiceRollImage()
    {
        return introCutsceneUI.rootVisualElement.Q<VisualElement>("DiceRollImage");
    }
    private VisualElement GetBadLuckImage()
    {
        return introCutsceneUI.rootVisualElement.Q<VisualElement>("BadLuckImage");
    }

    private Label GetCutsceneText()
    {
        return introCutsceneUI.rootVisualElement.Q<Label>("CutsceneText");
    }
}
