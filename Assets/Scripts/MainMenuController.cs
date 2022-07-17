using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{

    [SerializeField]
    private UIDocument mainMenuUI = null; 

    // Start is called before the first frame update
    void Start()
    {
        StartButtonHookup();
    }

    private void StartButtonHookup()
    {
        GetStartButton().clicked += () =>
        {
           SceneManager.LoadScene("IntroCutscene");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Button GetStartButton()
    {
        return mainMenuUI.rootVisualElement.Q<Button>("StartButton");
    }
}
