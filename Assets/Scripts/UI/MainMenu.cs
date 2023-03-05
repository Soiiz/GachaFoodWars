using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string trainingScene; // instruction room string name
    [SerializeField] string restScene; // rest string name 
    [SerializeField] PlayerProgression pprog;

    [Header("Sounds")]
    public AudioSource menuClick;

    private string _nextScene;

    public void PlayGame(){
        menuClick.Play();
        if (!pprog.TrainingComplete)
        {
            _nextScene = trainingScene;
            pprog.TrainingComplete = true;
        }
        else
        {
            _nextScene = restScene;
        }

        SceneManager.LoadScene(_nextScene);
        Time.timeScale = 1f;
        //GameObject.FindWithTag("BackgroundMusic").GetComponent<AudioSource>().enabled = true;
    }

    public void QuitGame(){
        menuClick.Play();
        Application.Quit();
    }
}
