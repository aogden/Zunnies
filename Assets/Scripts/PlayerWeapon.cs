using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour {

	public const float IMPACT_OFFSET = 0.01f;
	public const float SHOOT_COOLDOWN = 0.15f;
	public const float ACCURACY_DELTA = 3.0f;

	public AudioClip shootSound;
	public GameObject bulletImpact;
	public GameObject bloodSpray;

	private float _shootCooldown;
	private Camera _cam;
	private Player _player;

	#region Unity Lifecycle
	void Start () {
		
		// Get reference to Camera
		_cam = Camera.main;
		if (_cam == null){
			Debug.LogError("PlayerWeapon: Start: could not find required Camera component");
			return;
		}
		
		_player = GetComponent<Player>();
		if(_player == null)
		{
			Debug.Log("PlayerWeapon: Start: No player component!");
		}
	}
	void Update () {
		// Press spacebar to shoot
		if (Input.GetKey(KeyCode.Space)) {
			if(_player != null && _player.IsAlive())
			{
				Shoot();
			}
			_player.OnShoot();
		}

		_shootCooldown -= Time.deltaTime;
	}
	#endregion
	

	#region Gun Helpers
	private void Effect(Vector3 position, GameObject effectObject){
		GameObject effect = GameObject.Instantiate(effectObject) as GameObject;
		effect.transform.position = position;
	}
	private Quaternion GetOffsetQuaternion(float radius, float angleInRadians){
		Quaternion xQuaternion = Quaternion.AngleAxis(Mathf.Sin (angleInRadians) * radius, Vector3.up);
		Quaternion yQuaternion = Quaternion.AngleAxis(Mathf.Cos (angleInRadians) * radius, Vector3.left);
		return xQuaternion * yQuaternion;
	}
	private void Shoot(){
		
		// Early return if we are cooling down
		if (_shootCooldown < 0.0f) {
			_shootCooldown = SHOOT_COOLDOWN;
		} else {
			return;
		}

		// Calculate shoot direction
		Quaternion shootRotation = _cam.transform.rotation;
		float angle = Random.value * Mathf.PI * 2.0f;
		float radius = Mathf.Sqrt(Random.value) * ACCURACY_DELTA; 
		Vector3 shootVector = (shootRotation * GetOffsetQuaternion(radius, angle)) * Vector3.forward;

		// Play gunshot sound
		AudioSource.PlayClipAtPoint(shootSound, this.transform.position);

		// Do Raycast and find collision point
		RaycastHit hit = new RaycastHit ();
		bool collided = Physics.Raycast (_cam.transform.position, shootVector, out hit);
		if (!collided) {
			return;
		}

		// Damage the enemy
		bool hitEnemy = false;
		if (hit.collider.gameObject.tag == "Enemy") {
			try{
				Damageable e = hit.collider.gameObject.GetComponent<Damageable>();
				e.Damage(-hit.normal, hit.point);
				hitEnemy = true;
			}catch{
				Debug.LogError("PlayerWeapon: Shoot: not an enemy");
				return;
			}
		}

		// Create bullet impact effect
		Vector3 impactPoint = hit.point + hit.normal * IMPACT_OFFSET;
		if (hitEnemy){
			Effect (impactPoint, bloodSpray);
		}else{
			Effect (impactPoint, bulletImpact);
		}
	}
	#endregion
}
