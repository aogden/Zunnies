using UnityEngine;
using System.Collections;

public delegate void DamageEventHandler(Vector3 impactDirection, Vector3 impactPosition);
public delegate void ExplosionEventHandler(Vector3 explosionCenter, float explosionForce, float explosionRadius);
public delegate void DieEventHandler();

public class Damageable : MonoBehaviour {
	
	public event DamageEventHandler OnDamage;
	public event ExplosionEventHandler OnExplosion;
	public event DieEventHandler OnDie;
	
	public float MaxHealth = 20.0f;
	
	private float _currenthealth;
	
	void Awake()
	{
		_currenthealth = MaxHealth;
	}
	
	public float GetCurrentHealth()
	{
		return _currenthealth;
	}
	
	public bool IsAlive()
	{
		return _currenthealth > 0.0f;
	}
	
	public void Damage(Vector3 impactDirection, Vector3 impactPosition)
	{
		//report damage to event listeners
		if(OnDamage != null)
		{
			OnDamage(impactDirection, impactPosition);
		}
		
		_currenthealth = Mathf.Max(0.0f,_currenthealth - 1.0f);
		if(_currenthealth <= 0.0f)
		{
			Die();
		}
	}
	
	public void Explode(Vector3 explosionCenter, float explosionForce, float explosionRadius)
	{
		//report damage to event listeners
		if(OnExplosion != null)
		{
			OnExplosion(explosionCenter, explosionForce, explosionRadius);
		}
	}
	
	private void Die()
	{
		//report death to event listeners
		if(OnDie != null)
		{
			OnDie();
		}
	}
}

