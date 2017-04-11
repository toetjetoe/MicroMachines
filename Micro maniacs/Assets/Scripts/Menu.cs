using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private Animator anim;
    private bool controller;


    [Header("Buttons")]
    public GameObject playButton;
    public GameObject backButton;
    public Toggle controllerButton;

    public GameObject mainUI;
    public GameObject settingsUI;
    public EventSystem eventSystem;

    [Header("Loading")]
    private bool loading = false;
    private float loadingProgress = 0;
    public GameObject loadingScreen;
    public GameObject ControllerScreen;
    public GameObject KeyboardScreen;
    public Transform progressBar;

    private float progressScaleX;
    private Vector2 progressScaleYZ;

    private void Awake()
    {
        controllerButton.isOn = (PlayerPrefs.GetInt("Controller") > 0);
    }

    void Start () {
        ToggleUI(true, false);
        anim = GetComponent<Animator>();
        controller = controllerButton.isOn;

        progressScaleYZ = new Vector2(progressBar.localScale.y, progressBar.localScale.z);

        loadingScreen.SetActive(false);
    }

    private void Update()
    {
        if (loading)
        {
            loadingProgress = loadingProgress + 0.6f * Time.deltaTime;
            progressBar.GetComponent<Image>().fillAmount = loadingProgress / 10;

            if(loadingProgress >= 10)
            {
                SceneManager.LoadSceneAsync(1);
                loading = false;
            }
        }
    }

    private void ToggleUI(bool main, bool settings)
    {
        mainUI.SetActive(main);
        settingsUI.SetActive(settings);
        if (main)
        {
            eventSystem.SetSelectedGameObject(playButton);
        }
        if(settings)
        {
            eventSystem.SetSelectedGameObject(backButton);
        }
    }


    public void Play()
    {
        SaveSettings();
        loading = true;

        loadingScreen.SetActive(true);
        if (controller)
        {
            ControllerScreen.SetActive(true);
            KeyboardScreen.SetActive(false);
        }
        else
        {
            ControllerScreen.SetActive(false);
            KeyboardScreen.SetActive(true);
        }

        ToggleUI(false, false);
    }


    public void ToggleSettings(bool moveToSettings)
    {
        if (moveToSettings)
        {
            anim.SetBool("Settings", true);
            ToggleUI(false, true);
        }
        else
        {
            anim.SetBool("Settings", false);
            ToggleUI(true, false);
        }
    }


    public void Exit()
    {
        SaveSettings();
        Application.Quit();
    }

    public void Controller()
    {
        if (controller)
        {
            controller = false;
        }
        else
        {
            controller = true;
        }
    }

    private void SaveSettings()
    {
        if (controller)
        {
            PlayerPrefs.SetInt("Controller", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Controller", 0);
        }
    }
}
