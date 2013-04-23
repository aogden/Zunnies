using UnityEngine;
using System.Collections;

public class PressQToQuit : MonoBehaviour {
	void Update () {

		if (Input.GetKey (KeyCode.Q)) {
			Application.Quit();
		}
	}
}
