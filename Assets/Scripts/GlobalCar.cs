using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GlobalCar : MonoBehaviour {

    public static int CarType; //1=Red, 2=Blue, //3=Green
    public GameObject TrackWindow;

    public void RedCar ()
    {
        bool playOutput = EditorUtility.DisplayDialog("Select Car", "Are you sure you want to select the red car?", "Yes", "No");
        if (playOutput)
        {
            CarType = 1;
            TrackWindow.SetActive(true);
        }
        
        else 
            EditorUtility.DisplayDialog("Canceled", "You canceled the selection.", "OK");
 
    }

    public void BlueCar ()
    {
        bool playOutput = EditorUtility.DisplayDialog("Select Car", "Are you sure you want to select the blue car?", "Yes", "No");
        if (playOutput)
        {
            CarType = 2;
            TrackWindow.SetActive(true);
        }

        else
            EditorUtility.DisplayDialog("Canceled", "You canceled the selection.", "OK");
    }


}
