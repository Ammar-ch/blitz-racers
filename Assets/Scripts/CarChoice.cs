using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarChoice : MonoBehaviour {

    //1=Red, 2=Blue
    public GameObject RedBody;
    public GameObject BlueBody;
    public int CarImport;

	void Start () {
        CarImport = GlobalCar.CarType;
        if (CarImport == 1)
        {
            RedBody.SetActive(true);
        }

        if (CarImport == 2)
        {
            BlueBody.SetActive(true);
        }

        if (CarImport == 0)
        {
            RedBody.SetActive(true);
            BlueBody.SetActive(true);
        }
    }

}
