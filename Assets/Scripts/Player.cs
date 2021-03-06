using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	// Rendering Constants
	public const float NEAR_Z = 0.3f;
	public const float DEFAULT_VIEW_DISTANCE = 1000.0f;
	public const float FOG_DEPTH = 800.0f;
	
	// Input Constants
	public const float SENSITIVITY_X = 4.0f;
	public const float SENSITIVITY_Y = 4.0f;
	public const float MAX_VERTICAL_LOOK_DEGREES = 40.0f;
	
	// Player Constants
	public const float MOVE_SPEED = 75.0f;
	public const float PLAYER_HEIGHT = 16.0f;
	
	private Camera _cam;
	private float _rotX;
	private float _rotY;
	private Quaternion _initialCameraRot;
	
	private Damageable _damageableComponent;
	private float _score;
	private Vector3 _spawnLocation;
	private Quaternion _spawnRotation;
	
	#region Unity Lifecycle
	void Start () {

		// Get required components and default camera orientation
		_cam = Camera.main;
		if (_cam == null){
			Debug.LogError("Player: Start: _cam is null");
			return;
		}

		_initialCameraRot = _cam.transform.rotation;

		// Capture the mouse
		Screen.showCursor = false;
		Screen.lockCursor = true;

		// Set default render settings
		DefaultFog();
		DefaultClipping();
		
		//Attach our damage listeners
		_damageableComponent = GetComponent<Damageable>();
		if(_damageableComponent != null)
		{
			_damageableComponent.OnDamage += Damage;
			_damageableComponent.OnDie += Die;
		}
		
		//save our spawn place for restart
		_spawnLocation = transform.position;
		_spawnRotation = transform.rotation;
		
		// Fall to ground if Camera was placed too high
		Move(Vector3.up, 0.0f);	
	}

	void Update () {
		if(_damageableComponent != null && !_damageableComponent.IsAlive())
		{
			return;
		}
		
		// Let the player look, shoot, and move
		UpdateLookRotation ();
		UpdatePosition ();
	}
	#endregion

	#region Render Settings
	private void DefaultFog(){
		
		// Make fog respect the player's view distance
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color (0.25f, 0.3f, 0.35f);
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogEndDistance = DEFAULT_VIEW_DISTANCE;
		RenderSettings.fogStartDistance = DEFAULT_VIEW_DISTANCE - FOG_DEPTH;
	}
	private void DefaultClipping(){
		
		// Adjust the clipping to match the player's view distance
		_cam.near = NEAR_Z;
		_cam.far = DEFAULT_VIEW_DISTANCE;
	}
	#endregion

	#region Move Helpers
	private void Move(Vector3 direction, float speed){
		if (Movement.Move(this.gameObject, direction, speed)){
			this.transform.position += new Vector3(0, PLAYER_HEIGHT, 0);
		}
	}
	private void UpdatePosition(){
		
		// Check keyboard input
		Vector3 moveDirection = new Vector3();
		if (Input.GetKey (KeyCode.W)) {
			moveDirection += _cam.transform.forward;
		}
		if (Input.GetKey (KeyCode.S)) {
			moveDirection -= _cam.transform.forward;
		}
		if (Input.GetKey (KeyCode.D)) {
			moveDirection += _cam.transform.right;
		}
		if (Input.GetKey (KeyCode.A)) {
			moveDirection -= _cam.transform.right;
		}
		if (moveDirection.magnitude > 0.0f){
			moveDirection.Normalize();
			Move(moveDirection, MOVE_SPEED);
		}
	}
	private void UpdateLookRotation(){
		
		// Determine rotation based on mouse movement
		_rotX = (_rotX + (Input.GetAxis("Mouse X") * SENSITIVITY_X)) % 360;
		_rotY = Mathf.Clamp(_rotY + (Input.GetAxis("Mouse Y") * SENSITIVITY_Y), -MAX_VERTICAL_LOOK_DEGREES, MAX_VERTICAL_LOOK_DEGREES);
		
		// Update camera orientation
		Quaternion xQuaternion = Quaternion.AngleAxis(_rotX, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis(_rotY, Vector3.left);
		_cam.transform.localRotation = _initialCameraRot * xQuaternion * yQuaternion;
	}
	#endregion
	
	#region EventHandlers
	private void Damage(Vector3 impactDirection, Vector3 impactPosition){
		Debug.Log("PLAYER DAMAGE");
	}
	private void Die()
	{
		gameObject.AddComponent<GameOverDialog>();
		EndGame();
	}
	public void OnKill(Enemy killedEnemy)
	{
		_score ++;
	}
	public void OnShoot()
	{
		if(!IsAlive())
		{
			RestartGame();
		}
	}
	#endregion
	
	#region Accessors
	public float GetScore()
	{
		return _score;
	}
	public bool IsAlive()
	{
		return _damageableComponent.IsAlive();
	}
	#endregion
	
	#region Gameplay Helpers
	private void EndGame()
	{
		Enemy[] enemies = (Enemy[])GameObject.FindObjectsOfType(typeof(Enemy));
		foreach(Enemy enemy in enemies)
		{
			Destroy(enemy.gameObject);
		}
	}
	private void RestartGame()
	{
		GameOverDialog dialog = GetComponent<GameOverDialog>();
		if(dialog != null)
		{
			Destroy(dialog);
		}
		_damageableComponent.Restart();
		transform.position = _spawnLocation;
		transform.rotation = _spawnRotation;
	}
	#endregion
}
