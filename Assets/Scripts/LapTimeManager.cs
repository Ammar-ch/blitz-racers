using UnityEngine;
using UnityEngine.UI;

public class LapTimeManager : MonoBehaviour
{

    public static int MinuteCount;
    public static int SecondCount;
    public static float MilliCount;
    public static string MilliDisplay;

    public GameObject MinuteBox;
    public GameObject SecondBox;
    public GameObject MilliBox;


    void Update()
    {
        // Time.deltaTime gives the time in seconds since the last frame was drawn.
        MilliCount += Time.deltaTime * 10; // Accumulate MilliCount in tenths of a second for higher precision.

        // Convert MilliCount to a string without any decimal points for clearer display.
        MilliDisplay = MilliCount.ToString("F0");

        // Display the updated MilliCount in the UI by setting the text of the MilliBox.
        MilliBox.GetComponent<Text>().text = MilliDisplay; // Show the current value of MilliCount in MilliBox.


        if (MilliCount >= 10)   // Reset MilliCount and increment SecondCount when MilliCount reaches 10
        {
            MilliCount = 0;
            SecondCount += 1;
        }

        if (SecondCount <= 9)
        {
            SecondBox.GetComponent<Text>().text = "0" + SecondCount + ".";
        }
        else
        {
            SecondBox.GetComponent<Text>().text = "" + SecondCount + ".";
        }

        if (SecondCount >= 60)
        {
            SecondCount = 0;
            MinuteCount += 1;
        }

        if (MinuteCount <= 9)
        {
            MinuteBox.GetComponent<Text>().text = "0" + MinuteCount + ":";
        }
        else
        {
            MinuteBox.GetComponent<Text>().text = "" + MinuteCount + ":";
        }

    }
}
