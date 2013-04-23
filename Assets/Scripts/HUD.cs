using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public Texture2D crosshair;

	void OnGUI() {
		GUI.Label(new Rect(0, 0, 120, 120), "WASD to move \nMouse to aim \nSpacebar to shoot"); 
		float crosshairSize = 128.0f;
		float left = (Screen.width - crosshairSize) * 0.5f;
		float bottom = (Screen.height - crosshairSize) * 0.5f;
		GUI.DrawTexture(new Rect(left, bottom, crosshairSize, crosshairSize), crosshair);
	}
}
