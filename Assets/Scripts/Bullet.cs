using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public float Speed = 50f;
	public int Power = 1;
	public float Radius = 0.25f;
	
	public LayerMask HitMask;
	
	public Collider FiringObject
	{
		get; set;
	}
	
	
	private float lifetime = 0f;
	
	
	void Update()
	{
		float delta = Speed*Time.deltaTime;
		
		Ray ray = new Ray(transform.position - transform.forward, transform.forward * delta);
		
		RaycastHit hit = new RaycastHit();
		if(Physics.SphereCast(ray,Radius,out hit,delta,HitMask.value))
		{
			if(hit.collider!=FiringObject)
			{
				hit.collider.gameObject.SendMessage("OnWasHit",Power,SendMessageOptions.DontRequireReceiver);
			//	Debug.Log ("Hit "+hit.collider.gameObject.name);
				if(hit.collider.gameObject.tag != "Ground")
				{
			//		Destroy(hit.collider.gameObject);
				}
				GetComponentInChildren<Renderer>().enabled = false;
				Destroy (gameObject);
			}
			else
			{
				transform.position += transform.forward * delta;
			}
		}
		else
		{
			transform.position += transform.forward * delta;
		}
		
		lifetime += Time.deltaTime;
		
		if(lifetime>10f)
		{
			Destroy(gameObject);
		}
	}
	
	
}
