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
            
            SetOpacity(GetDevilImage(), 0.0f);
            SetOpacity(GetCutsceneText(), 0.0f);
            SetOpacity(GetUIBackGround(), 0.0f);

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

    IEnumerator ChangeTextInAndOut(Label textLabel, float opacityChangeTime, string text)
    {
        textLabel.text = text;
        yield return StartCoroutine(FadeIn(textLabel, opacityChangeTime));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(FadeOut(textLabel, opacityChangeTime));
    }

    IEnumerator CutsceneSequence()
    {
        var devilImage = GetDevilImage();
        var textLabel = GetCutsceneText();
        var uiBackground = GetUIBackGround();

        SetOpacity(devilImage, 0.0f);
        SetOpacity(textLabel, 0.0f);
        SetOpacity(uiBackground, 1.0f);

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(ChangeTextInAndOut(textLabel, 2.0f, "Jack, roll your last dice, betting everything on this..."));

        yield return StartCoroutine(FadeIn(devilImage, 2.0f));

        yield return StartCoroutine(ChangeTextInAndOut(textLabel, 2.0f, "Looks like your soul...\nIs Mine"));

        yield return StartCoroutine(FadeOut(devilImage, 1.5f));

        yield return StartCoroutine(FadeOut(uiBackground, 1.5f));

        yield return new WaitForSeconds(2.0f);

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

    private Label GetCutsceneText()
    {
        return introCutsceneUI.rootVisualElement.Q<Label>("CutsceneText");
    }
}
