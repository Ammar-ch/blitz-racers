using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dreamcar01Track : MonoBehaviour
{

	public GameObject TheMarker;
	public GameObject[] Markers; // Array to hold all marker GameObjects

	public int MarkTracker;

	void Update()
	{
		// Check if MarkTracker is within the bounds of the Markers array
		if (MarkTracker >= 0 && MarkTracker < Markers.Length)
		{
			SetMarkerTransform(Markers[MarkTracker]);
		}
	}

	void SetMarkerTransform(GameObject marker)
	{
		// Set both position and rotation
		TheMarker.transform.position = marker.transform.position;
		TheMarker.transform.rotation = marker.transform.rotation;
	}

	IEnumerator OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.tag == "DreamCar01")
		{
			this.GetComponent<BoxCollider>().enabled = false;
			MarkTracker += 1;
			// Loop back to the start if MarkTracker exceeds the number of markers
			if (MarkTracker >= Markers.Length)
			{
				MarkTracker = 0;
			}
			yield return new WaitForSeconds(1);
			this.GetComponent<BoxCollider>().enabled = true;
		}
	}
}
