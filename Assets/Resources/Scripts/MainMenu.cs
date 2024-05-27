using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject Options;
    public GameObject Levels;

    public AudioSource soundButton;

    public Button level2B;
    public Button level3B;
    public Button level4B;
    public Button level5B;
    int levelComplete;

    public static MainMenu instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (PlayerPrefs.GetInt("StateMenu") == 1)
        {
            MoveMenu(1);
            PlayerPrefs.SetInt("StateMenu", 0);
        }
        levelComplete = PlayerPrefs.GetInt("LevelComplete");
        level2B.interactable = false;
        level3B.interactable = false;
        level4B.interactable = false;
        level5B.interactable = false;

        switch (levelComplete)
        {
            case 1:
                level2B.interactable = true;
                break;
            case 2:
                level2B.interactable = true;
                level3B.interactable = true;
                break;
            case 3:
                level2B.interactable = true;
                level3B.interactable = true;
                level4B.interactable = true;
                break;
            case 4:
                level2B.interactable = true;
                level3B.interactable = true;
                level4B.interactable = true;
                level5B.interactable = true;
                break;
        }
    }


    public void MoveMenu(int i)
    {
        switch (i)
        {
            case 0:
                Menu.SetActive(true);
                Options.SetActive(false);
                Levels.SetActive(false);
                break;
            case 1:
                Menu.SetActive(false);
                Options.SetActive(false);
                Levels.SetActive(true);
                break;
            case 2:
                Menu.SetActive(false);
                Options.SetActive(true);
                Levels.SetActive(false);
                break;
            }
        
    }

    public void OnButtonDown (GameObject button) 
    {
        soundButton.Play();
        button.transform.localScale = new Vector3(0.9f,0.9f, 1f);
    }
    public void OnButtonUp (GameObject button)
    {
        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void Reset()
    {
        level2B.interactable = false;
        PlayerPrefs.DeleteAll(); ;
    }

    public void LoadLevel(int level) 
    {
        SceneLoading.LoadScene(level);
    }

    public void QuitGame() 
    {
        Application.Quit(); 
    }
}
