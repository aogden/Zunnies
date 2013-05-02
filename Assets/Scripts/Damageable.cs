using UnityEngine;
using System.Collections;

public delegate void DamageEventHandler(Vector3 impactDirection, Vector3 impactPosition);
public delegate void ExplosionEventHandler(Vector3 explosionCenter, float explosionForce, float explosionRadius);

public class Damageable : MonoBehaviour {
	
	public event DamageEventHandler OnDamage;
	public event ExplosionEventHandler OnExplosion;
	
	public void Damage(Vector3 impactDirection, Vector3 impactPosition)
	{
		//report damage to event listeners
		if(OnDamage != null)
		{
			OnDamage(impactDirection, impactPosition);
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
}

