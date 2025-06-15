using UnityEngine;
using UnityEngine.UI;

public class LoadLapTime : MonoBehaviour
{
    public int MinCount;
    public int SecCount;
    public float MilliCount;
    public GameObject MinDisplay;
    public GameObject SecDisplay;
    public GameObject MilliDisplay;

    void Start()
    {
        //Load saved
        MinCount = PlayerPrefs.GetInt("MinSave");
        SecCount = PlayerPrefs.GetInt("SecSave");
        MilliCount = PlayerPrefs.GetFloat("MilliSave");

        // Set the text of MinuteDisplay to show 
        MinDisplay.GetComponent<Text>().text = MinCount.ToString("00") + ":";
        SecDisplay.GetComponent<Text>().text = SecCount.ToString("00") + ".";
        MilliDisplay.GetComponent<Text>().text = MilliCount.ToString("00.0"); 
    }
}
