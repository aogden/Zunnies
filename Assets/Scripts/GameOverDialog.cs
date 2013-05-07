using UnityEngine;
using System.Collections;

public class GameOverDialog : MonoBehaviour {
	
	void OnGUI() {
		
		Player playerComponent = GetComponent<Player>();
		if(playerComponent == null)
		{
			Debug.Log ("GameOverDialog: OnGUI: failed to get player component");
			return;
		}
		
		GUI.Box(new Rect(Screen.width / 2.0f - 200, Screen.height/2.0f - 200, 400, 400),"");
		
		GUIStyle style = new GUIStyle();
		style.fontSize = 40;
		style.alignment = TextAnchor.MiddleCenter;
		int kills = (int)playerComponent.GetScore();
		string text = "GAME OVER\nYOU GOT "+kills+" KILL"+(string)(kills != 1 ? "S": ""); //make KILL plural if you have other than 1 kill
		text += "\n\nShoot to restart";
		GUI.Label(new Rect(Screen.width / 2.0f - 200, Screen.height/2.0f - 200, 400, 400),text,style);
	}
}
