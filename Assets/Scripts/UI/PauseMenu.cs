using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource menuClick;
    protected Canvas pauseMenu;
    //public GameObject restartMenu;
    public static bool isPaused;

    void Start()
    {

        pauseMenu = GetComponent<Canvas>();
        pauseMenu.enabled = false;
        //restartMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        menuClick.Play();
        pauseMenu.enabled = true;
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        menuClick.Play();
        pauseMenu.enabled = false;
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        menuClick.Play();
        Time.timeScale = 1f;
        isPaused = false;
        GameObject.FindWithTag("BackgroundMusic").GetComponent<AudioSource>().enabled = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        menuClick.Play();
        Application.Quit();
    }
}
