using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public Texture2D crosshair;
	
	private Damageable _playerDamageable;
	private Player _player;
	
	void Start()
	{
		_playerDamageable = GetComponent<Damageable>();
		if(_playerDamageable == null)
		{
			Debug.Log("HUD: Start: Error getting player damageable component");
		}
		_player = GetComponent<Player>();
		if(_player == null)
		{
			Debug.Log("HUD: Start: Error getting player component");
		}
	}
	
	void OnGUI() {
		GUI.Label(new Rect(0, 0, 120, 120), "WASD to move \nMouse to aim \nSpacebar to shoot"); 
		float crosshairSize = 128.0f;
		float left = (Screen.width - crosshairSize) * 0.5f;
		float bottom = (Screen.height - crosshairSize) * 0.5f;
		GUI.DrawTexture(new Rect(left, bottom, crosshairSize, crosshairSize), crosshair);
		
		if(_playerDamageable != null)
		{
			GUIStyle style = new GUIStyle();
			
			float healthPercent = _playerDamageable.GetCurrentHealth() / _playerDamageable.MaxHealth;
			if(healthPercent > 0.7f)
			{
				style.normal.textColor = Color.green;
			}
			else if(healthPercent > 0.25f)
			{
				style.normal.textColor = Color.yellow;
			}
			else
			{
				style.normal.textColor = Color.red;
			}
			GUI.Label(new Rect(Screen.width - 120, 0, 120, 20), "Health: "+_playerDamageable.GetCurrentHealth(),style);
		}
		
		if(_player != null)
		{
			GUI.Label(new Rect(Screen.width - 120, 20, 120, 20), "Score: "+_player.GetScore());
		}
	}
}
