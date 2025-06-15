using UnityEngine;
using UnityEngine.UI;

public class LapComplete : MonoBehaviour
{
    public GameObject LapCompleteTrig;
    public GameObject HalfLapTrig;

    public GameObject MinuteDisplay;
    public GameObject SecondDisplay;
    public GameObject MilliDisplay;

  //public GameObject LapTimeBox;

    void OnTriggerEnter()
    {
        // Calculate current lap time in seconds
        float currentLapTime = LapTimeManager.MinuteCount * 60 + LapTimeManager.SecondCount + LapTimeManager.MilliCount / 1000.0f;
        // Calculate saved lap time in seconds
        float savedLapTime = PlayerPrefs.GetInt("MinSave") * 60 + PlayerPrefs.GetInt("SecSave") + PlayerPrefs.GetFloat("MilliSave") / 1000.0f;

        // Check if the current lap time is a new record or no time has been saved yet.
        if (currentLapTime < savedLapTime || savedLapTime == 0)
        {
            PlayerPrefs.SetInt("MinSave", LapTimeManager.MinuteCount);
            PlayerPrefs.SetInt("SecSave", LapTimeManager.SecondCount);
            PlayerPrefs.SetFloat("MilliSave", LapTimeManager.MilliCount);

            MinuteDisplay.GetComponent<Text>().text = LapTimeManager.MinuteCount.ToString("00") + ":";
            SecondDisplay.GetComponent<Text>().text = LapTimeManager.SecondCount.ToString("00") + ".";
            MilliDisplay.GetComponent<Text>().text = LapTimeManager.MilliCount.ToString();
        }

        LapTimeManager.MinuteCount = 0;
        LapTimeManager.SecondCount = 0;
        LapTimeManager.MilliCount = 0;

        HalfLapTrig.SetActive(true);
        LapCompleteTrig.SetActive(false);
    }
}
