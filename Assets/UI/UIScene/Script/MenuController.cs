using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject titlePanel;
    
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button gameStartButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button exitButton;
    
    [Header("Options")]
    public GameObject optionsPanel;
    
    [Header("Intro")]
    public GameObject introPanel;
    
    // TODO: Animation?
    public Animator introAnimator;
    

    private enum Panels { Title, MainMenu, Options, Intro};

    private Panels _State;
    
    // Start is called before the first frame update
    void Start()
    {
        SwitchToPanel(Panels.Title);
        gameStartButton.onClick.AddListener(GameStart);
        optionsButton.onClick.AddListener(Options);
        // creditsButton.onClick.AddListener(Credits);
        exitButton.onClick.AddListener(Exit);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_State==Panels.Title&&Input.anyKey)
        {
            SwitchToPanel(Panels.MainMenu);
        }
    }

    private void SwitchToPanel(Panels panel)
    {
        switch (panel)
        {
            case Panels.Title:
                _State = Panels.Title;
                titlePanel.SetActive(true);
                mainMenuPanel.SetActive(false);
                optionsPanel.SetActive(false);
                introPanel.SetActive(false);
                break;
            case Panels.MainMenu:
                _State = Panels.MainMenu;
                titlePanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                optionsPanel.SetActive(false);
                introPanel.SetActive(false);
                break;
            case Panels.Options:
                _State = Panels.Options;
                titlePanel.SetActive(false);
                mainMenuPanel.SetActive(false);
                optionsPanel.SetActive(true);
                introPanel.SetActive(false);
                break;
            case Panels.Intro:
                _State = Panels.Intro;
                titlePanel.SetActive(false);
                mainMenuPanel.SetActive(false);
                optionsPanel.SetActive(false);
                introPanel.SetActive(true);
                break;
        }
    }
    
    private void GameStart()
    {
        SwitchToPanel(Panels.Intro);
        // introAnimator.SetTrigger("PlayIntro");
        StartCoroutine(LoadGameScene());
    }

    public void MainMenu()
    {
        SwitchToPanel(Panels.MainMenu);
    }
    
    private void Options()
    { 
        SwitchToPanel(Panels.Options);
    }
    
    private void Credits()
    {
        // TODO: make Credits panel
        // SwitchToPanel(Panels.Credits);
    }
    
    public void Exit()
    { 
        Application.Quit();
    }
    
    // Load the game scene asynchronously
    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(5f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Scene1");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // TODO: Load indicator?
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null;
        }
        Debug.Log("Loading complete");
        // yield return new WaitUntil(() => !introAnimator.GetCurrentAnimatorStateInfo(0).IsName("IntroAnimation"));

    }
}
