using UnityEngine;
using UnityStandardAssets.Vehicles.Car; 

public class CarControlActive : MonoBehaviour
{
    public GameObject MyCar;
    public GameObject Dreamcar01;
    public GameObject Dreamcar02;

    void Start()
    {
        MyCar.GetComponent<CarController>().enabled = true;

        Dreamcar01.GetComponent<CarController>().enabled = true;
        Dreamcar01.GetComponent<CarAIControl>().enabled = true;


        Dreamcar02.GetComponent<CarController>().enabled = true;
        Dreamcar02.GetComponent<CarAIControl1>().enabled = true;
    }
}
