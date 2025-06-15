
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void AIBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LapTimer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has Quit the game");
    }

    public void ResetTime()
    {
        PlayerPrefs.SetInt("MinSave", 0);
        PlayerPrefs.SetInt("SecSave", 0);
        PlayerPrefs.SetFloat("MilliSave", 0.0f);
        Debug.Log("Time has been reset");
    }
}
