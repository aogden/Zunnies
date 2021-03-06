using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	public const float MOVE_SPEED = 32.0f;
	public const float CORPSE_REMOVAL_DELAY = 5.0f;
	
	private Player _player;
	private int _updates;
	private float _timeDead;
	private bool _dead;
	
	private Damageable _damageableComponent;
	
	#region Unity Lifecycle
	void Awake(){
		this.GetComponent<MeshRenderer>().enabled = false;
		this.collider.enabled = true;
		this.rigidbody.angularDrag = 0.15f;
		this.rigidbody.drag = 1000.0f;
	}
	void Start () {
		
		// Get a reference to the Player. So we can chase him
		_timeDead = Random.value * CORPSE_REMOVAL_DELAY * 0.33f;
		_player = (Player)GameObject.FindObjectOfType(typeof(Player));
		if (_player == null){
			Debug.LogError("Enemy: Start: cant find player");
			return;
		}
		
		//Attach our damage listeners
		_damageableComponent = GetComponent<Damageable>();
		if(_damageableComponent != null)
		{
			_damageableComponent.OnDamage += Damage;
			_damageableComponent.OnExplosion += Explode;
			_damageableComponent.OnDie += Die;
		}

		// Make sure this doesn't spawn in the air
		MoveTowardsPlayer();
	}

	void LateUpdate (){

		// If dead, do nothing
		if (_dead){
			return;
		}

		// Let the physics engine take over again
		this.rigidbody.isKinematic = false;
	}

	void Update () {

		// Remove corpse after a timeout
		if (_dead){
			_timeDead += Time.deltaTime;
			if (_timeDead > CORPSE_REMOVAL_DELAY){
				GameObject.Destroy(this.gameObject);
			}
			return;
		}

		// Make kinematic so we can move it around without breaking thinfgs
		this.rigidbody.isKinematic = true;

		// Move towards player
		MoveTowardsPlayer();

		// Make visible again
		_updates++;
		if (_updates == 2){
			this.GetComponent<MeshRenderer>().enabled = true;
		}
	}
	#endregion
	
	#region Internal Helpers
	private void MoveTowardsPlayer(){

		// Early return if there is no player
		if (_player == null){
			return;
		}
		
		// Chase after player
		Vector3 directionToPlayer = _player.transform.position - this.transform.position;
		directionToPlayer.Normalize();
		Movement.Move(this.gameObject, directionToPlayer, MOVE_SPEED);
	}
	private void Ragdoll(){
		this.rigidbody.constraints = RigidbodyConstraints.None;
		this.rigidbody.useGravity = true;
		this.rigidbody.drag = 0.1f;
	}
	private void AnimateBulletImpact(Vector3 impactDirection, Vector3 impactPosition){
		Ragdoll();
		this.rigidbody.AddForceAtPosition(impactDirection * 3000, impactPosition);
	}
	private void AnimateExplosion(Vector3 center, float force, float radius){
		Ragdoll();
		this.rigidbody.AddExplosionForce(force, center, radius);
	}
	private void Die()
	{
		if(_player != null)
		{
			_player.OnKill(this);
		}
		_dead = true;
		if(_damageableComponent != null)
		{
			_damageableComponent.OnDamage -= Damage;
			_damageableComponent.OnExplosion -= Explode;
			_damageableComponent.OnDie -= Die;
		}
	}
	#endregion

	#region EventHandlers
	private void Damage(Vector3 impactDirection, Vector3 impactPosition){
		AnimateBulletImpact(impactDirection, impactPosition);
	}
	private void Explode(Vector3 explosionCenter, float explosionForce, float explosionRadius){
		AnimateExplosion(explosionCenter, explosionForce, explosionRadius);
	}
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "MainCamera" && _damageableComponent.IsAlive())
		{
			other.gameObject.GetComponent<Damageable>().Damage(Vector3.zero,Vector3.zero);
		}
	}
	#endregion
}
