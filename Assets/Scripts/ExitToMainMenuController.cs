using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ExitToMainMenuController : MonoBehaviour
{
    [SerializeField]
    private UIDocument exitToMainMenuUI = null; 

    // Start is called before the first frame update
    void Start()
    {
        HideUI();
        ButtonHookup();
    }

    void OnDisable()
    {
        //Reset incase we left timescale bad
        Time.timeScale = 1.0f;
    }

    private void ButtonHookup()
    {
        GetExitButton().clicked += () =>
        {
           SceneManager.LoadScene("MainMenu");
        };
        GetCancelButton().clicked += () =>
        {
            HideUI();
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ShowUI(); 
        }
    }

    private void HideUI()
    {
        GetUIVisualElement().style.opacity = 0.0f;
        Time.timeScale = 1.0f;
        GetExitButton().pickingMode = PickingMode.Ignore;
        GetCancelButton().pickingMode = PickingMode.Ignore;
    }  

    private void ShowUI()
    {
        GetUIVisualElement().style.opacity = 1.0f;
        Time.timeScale = 0.0f;
        GetExitButton().pickingMode = PickingMode.Position;
        GetCancelButton().pickingMode = PickingMode.Position;
    }  

    private Button GetExitButton()
    {
        return exitToMainMenuUI.rootVisualElement.Q<Button>("ExitButton");
    }    

    private Button GetCancelButton()
    {
        return exitToMainMenuUI.rootVisualElement.Q<Button>("CancelButton");
    }    

    private VisualElement GetUIVisualElement()
    {
        return exitToMainMenuUI.rootVisualElement.Q<VisualElement>("BackGround");
    }
}
